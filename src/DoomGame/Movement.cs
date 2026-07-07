using DoomCore;

namespace DoomGame
{
    public static class Movement
    {
        // Simplified port of P_XYMovement for skeleton: attempt to move by momx/momy once.
        public static void Tick(Mobj mo)
        {
            if ((mo.MomX.Raw == 0) && (mo.MomY.Raw == 0))
                return;

            // If missile has large momentum, sub-step movement to avoid tunneling
            if (mo.IsMissile)
            {
                double speedRaw = Math.Sqrt((double)mo.MomX.Raw * mo.MomX.Raw + (double)mo.MomY.Raw * mo.MomY.Raw);
                int stepEstimate = (int)Math.Ceiling(speedRaw / Fixed.FRACUNIT);
                int steps = Math.Clamp(stepEstimate, 1, 8); // limit max substeps to 8

                int stepMomX = mo.MomX.Raw / steps;
                int stepMomY = mo.MomY.Raw / steps;

                for (int i = 0; i < steps; i++)
                {
                    var pstepx = new Fixed(mo.X.Raw + stepMomX);
                    var pstepy = new Fixed(mo.Y.Raw + stepMomY);

                    if (mo.TryMove(pstepx, pstepy))
                    {
                        mo.SetPosition(pstepx, pstepy);
                    }
                    else
                    {
                        // blocked: remove momentum and stop stepping
                        mo.MomX = new Fixed(0);
                        mo.MomY = new Fixed(0);
                        break;
                    }

                    // After each substep check for collision
                    Collision.CheckHit(mo);

                    if (mo.StateIndex == StateTable.S_NULL)
                        break; // missile removed
                }

                return;
            }

            // Non-missile: sub-step movement for large momentum to make motion smoother and avoid tunneling.
            double speedRawNM = Math.Sqrt((double)mo.MomX.Raw * mo.MomX.Raw + (double)mo.MomY.Raw * mo.MomY.Raw);
            int stepEstimateNM = (int)Math.Ceiling(speedRawNM / Fixed.FRACUNIT);
            int stepsNM = Math.Clamp(stepEstimateNM, 1, 4); // fewer substeps for non-missiles

            int stepMomXNM = stepsNM == 0 ? 0 : mo.MomX.Raw / stepsNM;
            int stepMomYNM = stepsNM == 0 ? 0 : mo.MomY.Raw / stepsNM;

            for (int i = 0; i < stepsNM; i++)
            {
                var pstepx = new Fixed(mo.X.Raw + stepMomXNM);
                var pstepy = new Fixed(mo.Y.Raw + stepMomYNM);

                if (mo.TryMove(pstepx, pstepy))
                {
                    mo.SetPosition(pstepx, pstepy);
                }
                else
                {
                    // blocked: stop further movement but keep momentum zeroed to avoid stuck sliding
                    mo.MomX = new Fixed(0);
                    mo.MomY = new Fixed(0);
                    break;
                }

                // Check collisions each substep
                Collision.CheckHit(mo);

                if (mo.StateIndex == StateTable.S_NULL)
                    break; // removed
            }

            // Keep overall momentum so movement is continuous across ticks (smoother). Do not clear MomX/MomY here.

        }
    }
}
