if(!MovementManager.InMovement)
{
	questObjective.Position = ObjectManager.Me.Position;
	//Logging.Write(questObjective.Hotspots[listP.Count-1].DistanceTo(ObjectManager.Me.Position).ToString());
	if (questObjective.Position.IsValid && questObjective.Hotspots[questObjective.Hotspots.Count-1].DistanceTo(ObjectManager.Me.Position) > 5f)
	{ 
		Logging.Write("enter objectif 1");
		MountTask.Mount();
		System.Threading.Thread.Sleep(2000);
		var listP = new List<Point>();
		listP.Add(ObjectManager.Me.Position);
		listP.AddRange(questObjective.Hotspots);
		MovementManager.Go(listP);
		while(MovementManager.InMovement && questObjective.Hotspots[questObjective.Hotspots.Count-1].DistanceTo(ObjectManager.Me.Position) > 5f)
		{
		    System.Threading.Thread.Sleep(100);
		}
		MovementManager.StopMove();
	}
	if (questObjective.Hotspots[questObjective.Hotspots.Count-1].DistanceTo(ObjectManager.Me.Position) <= 5f)
	{
		Logging.Write("Completed");
        questObjective.IsObjectiveCompleted = true; 
		return true;
    }
}