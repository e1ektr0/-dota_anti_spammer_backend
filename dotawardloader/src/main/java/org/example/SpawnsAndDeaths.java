package org.example;

import skadistats.clarity.event.Event;
import skadistats.clarity.event.EventListener;
import skadistats.clarity.event.Initializer;
import skadistats.clarity.event.Provides;
import skadistats.clarity.model.Entity;
import skadistats.clarity.model.FieldPath;
import skadistats.clarity.processor.entities.OnEntityCreated;
import skadistats.clarity.processor.entities.OnEntityDeleted;
import skadistats.clarity.processor.entities.OnEntityUpdated;
import skadistats.clarity.processor.entities.UsesEntities;
import skadistats.clarity.processor.runner.Context;

import java.util.HashMap;
import java.util.Map;

@UsesEntities
@Provides({ OnEntitySpawned.class })
public class SpawnsAndDeaths {

    private final Map<Integer, FieldPath> lifeStatePaths = new HashMap<>();
    private final Map<Integer, Integer> currentLifeState = new HashMap<>();

    private Event<OnEntitySpawned> evSpawned;

    @Initializer(OnEntitySpawned.class)
    public void initOnEntitySpawned(final Context ctx, final EventListener<OnEntitySpawned> eventListener) {
        init(ctx);
        evSpawned = ctx.createEvent(OnEntitySpawned.class, Entity.class);
    }

    @OnEntityCreated
    public void onCreated(Context ctx, Entity e) {
        clearCachedState(e);
        ensureFieldPathForEntityInitialized(e);
        FieldPath p = getFieldPathForEntity(e);
        if (p != null) {
            processLifeStateChange(e, p);
        }
    }

    @OnEntityDeleted
    public void onDeleted(Context ctx, Entity e) {
        clearCachedState(e);
    }

    @OnEntityUpdated
    public void onUpdated(Context ctx, Entity e, FieldPath[] fieldPaths, int num) {
        FieldPath p = getFieldPathForEntity(e);
        if (p != null) {
            for (int i = 0; i < num; i++) {
                if (fieldPaths[i].equals(p)) {
                    processLifeStateChange(e, p);
                    break;
                }
            }
        }
    }

    private void init(Context ctx) {
    }

    private void ensureFieldPathForEntityInitialized(Entity e) {
        Integer cid = e.getDtClass().getClassId();
        if (!lifeStatePaths.containsKey(cid)) {
            lifeStatePaths.put(cid, e.getDtClass().getFieldPathForName("m_lifeState"));
        }
    }

    private FieldPath getFieldPathForEntity(Entity e) {
        return lifeStatePaths.get(e.getDtClass().getClassId());
    }

    private void clearCachedState(Entity e) {
        currentLifeState.remove(e.getIndex());
    }

    private void processLifeStateChange(Entity e, FieldPath p) {
        int oldState = currentLifeState.containsKey(e.getIndex()) ? currentLifeState.get(e.getIndex()) : 2;
        int newState = e.getPropertyForFieldPath(p);
        if (oldState != newState) {
            currentLifeState.put(e.getIndex(), newState);
            switch(newState) {
                case 0:
                    if (evSpawned != null) {
                        evSpawned.raise(e);
                    }
                    break;

            }
        }
    }

}
