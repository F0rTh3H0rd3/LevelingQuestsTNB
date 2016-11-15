﻿
Point sternfathomPosition = new Point(-333.6571f, 6672.701f, 3.538227f);

if (sternfathomPosition.DistanceTo(ObjectManager.Me.Position) <= 5f)
    return true;

Point entrancePosition = new Point(-347.0213f, 6656.839f, 1.14944f);

MovementManager.Go(PathFinder.FindPath(entrancePosition));
while (MovementManager.InMovement)
    Thread.Sleep(100);

MovementManager.Go(new List<Point> { ObjectManager.Me.Position, sternfathomPosition });
while (MovementManager.InMovement)
    Thread.Sleep(100);

if (sternfathomPosition.DistanceTo(ObjectManager.Me.Position) <= 5f)
    return true;

Logging.Write("Unable to auto move to Sternfathom, you have to do it.");
return false;