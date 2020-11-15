package org.example;

import skadistats.clarity.processor.runner.AbstractFileRunner;
import skadistats.clarity.processor.runner.LoopController;
import skadistats.clarity.source.Source;

import java.io.IOException;

public class CustomRunner extends AbstractFileRunner {
    private final LoopController.Func controllerFunc = (upcomingTick) -> {
        if(_stop)
            return LoopController.Command.BREAK;
        if (!this.loopController.isSyncTickSeen()) {
            if (this.tick == -1) {
                this.startNewTick(0);
                if(_stop)
                    return LoopController.Command.BREAK;
            }

        } else {
            if (upcomingTick != this.tick) {
                if (upcomingTick != 2147483647) {
                    this.endTicksUntil(upcomingTick - 1);
                    if(_stop)
                        return LoopController.Command.BREAK;
                    this.startNewTick(upcomingTick);
                    if(_stop)
                        return LoopController.Command.BREAK;
                } else {
                    this.endTicksUntil(this.tick);
                    if(_stop)
                        return LoopController.Command.BREAK;
                }
            }

        }
        return LoopController.Command.FALLTHROUGH;
    };
    public static boolean _stop;

    public CustomRunner(Source s) throws IOException {
        super(s, s.readEngineType());
        _stop = false;
        this.loopController = new LoopController(this.controllerFunc);
    }

    public CustomRunner runWith(Object... processors) throws IOException {
        _stop = false;
        this.initAndRunWith(processors);
        return this;
    }

    public void stop() {
        _stop = true;
    }
}
