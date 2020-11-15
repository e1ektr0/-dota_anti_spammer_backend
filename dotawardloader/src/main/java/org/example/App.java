package org.example;

import com.google.gson.Gson;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import org.apache.commons.compress.compressors.CompressorException;
import org.apache.commons.compress.compressors.CompressorInputStream;
import org.apache.commons.compress.compressors.CompressorStreamFactory;
import org.bson.Document;
import skadistats.clarity.model.Entity;
import skadistats.clarity.model.FieldPath;
import skadistats.clarity.processor.entities.OnEntityCreated;
import skadistats.clarity.processor.entities.OnEntityUpdated;
import skadistats.clarity.processor.runner.Context;
import skadistats.clarity.source.InputStreamSource;
import skadistats.clarity.source.Source;

import java.io.BufferedInputStream;
import java.io.IOException;
import java.net.URL;
import java.util.List;
import java.util.stream.Collectors;

public class App {

    private static List<MatchWardResult> _results;
    private static Float time = 0f;
    private static CustomRunner _runner;

    @OnEntityCreated(classPattern = "CDOTA_Hero_.*|CDOTA_NPC_Observer_Ward")
    public void onEntityCreated2(Context ctx, Entity e) {
        Integer playerIndex = e.getProperty("m_nPlayerOwnerID");
        Integer X = e.getProperty("CBodyComponent.m_cellX");
        Integer Y = e.getProperty("CBodyComponent.m_cellY");
        final WardResult result = new WardResult();
        result.time = Math.round(time);
        result.obs = true;
        result.x = X;
        result.y = Y;

        result.vecx = getVecOrigin(e, "X");
        result.vecy = getVecOrigin(e, "Y");

        final MatchWardResult matchWardResult = _results.get(playerIndex);
        matchWardResult.radiant = playerIndex < 5;
        matchWardResult.results.add(result);
    }

    @OnEntityCreated(classPattern = "CDOTA_Hero_.*|CDOTA_NPC_Observer_Ward_TrueSight")
    public void onEntityCreated(Context ctx, Entity e) {
        Integer playerIndex = e.getProperty("m_nPlayerOwnerID");
        Integer X = e.getProperty("CBodyComponent.m_cellX");
        Integer Y = e.getProperty("CBodyComponent.m_cellY");
        final WardResult result = new WardResult();
        result.time = Math.round(time);
        result.obs = false;
        result.x = X;
        result.y = Y;

        result.vecx = getX(e, X);
        result.vecy = getY(e, Y);

        final MatchWardResult matchWardResult = _results.get(playerIndex);
        matchWardResult.radiant = playerIndex < 5;
        matchWardResult.results.add(result);
    }

    @OnEntityUpdated(classPattern = "CDOTAGamerulesProxy")
    public void onGameRulesProxyUpdate(Context context, Entity e, FieldPath[] fieldPaths, int num) {
        time = getRealGameTimeSeconds(e);
        if (time != null && time > 120)
            _runner.stop();
    }

    public static Float getRealGameTimeSeconds(Entity gameRulesProxyEntity) {

        Float TIME_EPS = (float) 0.01;

        Float gameTime = gameRulesProxyEntity.getProperty("m_pGameRules.m_fGameTime");
        if (gameTime == null)
            return null;

        Float preGameTime = gameRulesProxyEntity.getProperty("m_pGameRules.m_flPreGameStartTime");
        if (preGameTime <= TIME_EPS)
            return null;

        Float startTime = gameRulesProxyEntity.getProperty("m_pGameRules.m_flGameStartTime");
        if (startTime > TIME_EPS)
            return gameTime - startTime;

        return gameTime - (float) gameRulesProxyEntity.getProperty("m_pGameRules.m_flStateTransitionTime");
    }

    public static float getVecOrigin(Entity e, String n) {
        return e.getProperty("CBodyComponent.m_vec" + n);
    }

    public static double getX(Entity e, int X) {

        return X * 128.0 + getVecOrigin(e, "X");
    }

    public static double getY(Entity e, int Y) {
        return (Y * -128.0) - getVecOrigin(e, "Y") + 32768.0;
    }


    @org.jetbrains.annotations.NotNull
    private static List<MatchWardResult> ProcessMatch(Match match) throws IOException, CompressorException, InterruptedException {
        time = 0f;
        System.out.println("start " + match.match_id);
        _results = match.players.stream().map(x -> {
            final MatchWardResult matchWardResult = new MatchWardResult();
            matchWardResult.match_id = match.match_id;
            matchWardResult.account_id = x.account_id;
            matchWardResult.hero_id = x.hero_id;
            return matchWardResult;
        }).collect(Collectors.toList());

        CompressorInputStream input = getBufferedReaderForCompressedFile(match.match_id, match.replay_salt, match.cluster);
        // 1) create an input source from the replay
        Source source = new InputStreamSource(input);
        // 2) create a simple runner that will read the replay once
        _runner = new CustomRunner(source);
        // 3) create an instance of your processor
        App processor = new App();
        // 4) and hand it over to the runner
        _runner.runWith(processor);

        System.out.println("done " + match.match_id);
        return _results;
    }

    public static CompressorInputStream getBufferedReaderForCompressedFile(Long match_id, Long replay_salt, int cluster) throws IOException, CompressorException {
        String matchName = match_id + "_" + replay_salt + ".dem.bz2";
        String url = "http://replay" + cluster + ".valve.net/570/" + matchName;
        BufferedInputStream bis = new BufferedInputStream(new URL(url).openStream());
        CompressorInputStream input = new CompressorStreamFactory().createCompressorInputStream(bis);
        return input;
    }

    public static void main(String[] args) throws IOException, CompressorException {
        try (MongoClient mongoClient = MongoClients.create("mongodb://e1ekt0:secretPassword@194.87.103.72:27017/?authSource=dota2&readPreference=primary&appname=MongoDB%20Compass&ssl=false")) {
            final MongoDatabase db = mongoClient.getDatabase("dota2");
            final MongoCollection<Document> matches = db.getCollection("matches_analyze");
            final MongoCollection<Document> ward_results = db.getCollection("ward_results");
            while (true) {
                try {
                    Gson g = new Gson();
                    final Document mongoMatch = matches.find().limit(1).first();
                    if (mongoMatch == null) {
                        System.out.println("no matches");
                        Thread.sleep(5100);
                        continue;
                    }
                    final String json = mongoMatch.toJson();
                    Match p = g.fromJson(json, Match.class);
                    final List<MatchWardResult> matchWardResults = ProcessMatch(p);
                    for (MatchWardResult result : matchWardResults) {
                        if (result.results.stream().count() == 0)
                            continue;
                        final String jsonResult = g.toJson(result);
                        Document dbObject = Document.parse(jsonResult);
                        ward_results.insertOne(dbObject);
                    }
                    matches.deleteOne(mongoMatch);
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }
    }
}
