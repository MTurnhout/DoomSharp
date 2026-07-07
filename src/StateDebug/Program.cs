using System;
using DoomGame;
using DoomCore;

class Program {
    static void Main() {
        bool actionCalled = false;
        var s0 = new State { NextState = 1, Tics = 0, Sprite = 0, Frame = 0 };
        var s1 = new State { NextState = StateTable.S_NULL, Tics = 3, Sprite = 1, Frame = 2, Action = (m) => actionCalled = true };
        StateTable.States = new State[] { s0, s1 };
        var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
        bool present = m.SetState(0);
        // Debug helper: inspected values in local runs
    }
}
