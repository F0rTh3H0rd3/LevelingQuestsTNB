/* Run Lua On Target

Check if there is HotSpots in the objective */
if (questObjective.Hotspots.Count <= 0)
{
	/* Objective CSharpScript with script UseItemWithHotSpots requires valid "Hotspots" */
	Logging.Write("RunLuaOnTarget requires valid 'HotSpots'.");
	questObjective.IsObjectiveCompleted = true;
	return false;
}

WoWGameObject node = ObjectManager.GetNearestWoWGameObject(ObjectManager.GetWoWGameObjectById(questObjective.Entry));
WoWUnit unit = ObjectManager.GetNearestWoWUnit(ObjectManager.GetWoWUnitByEntry(questObjective.Entry, questObjective.IsDead), questObjective.IgnoreNotSelectable, questObjective.IgnoreBlackList,
	questObjective.AllowPlayerControlled);
Point pos = ObjectManager.Me.Position; /* Initialize or getting an error */
int q = QuestID; /* not used but otherwise getting warning QuestID not used */
uint baseAddress = 0;

/* If Entry found continue, otherwise continue checking around HotSpots */
if ((unit.IsValid && !nManagerSetting.IsBlackListedZone(unit.Position) && !nManagerSetting.IsBlackListed(unit.Guid)) ||
	(node.IsValid && !nManagerSetting.IsBlackListedZone(unit.Position) && !nManagerSetting.IsBlackListed(unit.Guid)))
{
	if (questObjective.IgnoreFight)
		nManager.Wow.Helpers.Quest.GetSetIgnoreFight = true;
	/* Entry found, GoTo */
	if (node.IsValid)
	{
		baseAddress = MovementManager.FindTarget(node, questObjective.Range);
	}
	if (unit.IsValid)
	{
		baseAddress = MovementManager.FindTarget(unit, questObjective.Range);
	}
	Thread.Sleep(500);

	if (MovementManager.InMovement)
		return false;
	if (questObjective.IgnoreNotSelectable)
	{
		if ((node.IsValid && node.GetDistance > questObjective.Range) || (unit.IsValid && unit.GetDistance > questObjective.Range))
			return false;
	}
	else
	{
		if (baseAddress <= 0)
			return false;
		if (baseAddress > 0 && ((node.IsValid && node.GetDistance > questObjective.Range) || (unit.IsValid && unit.GetDistance > questObjective.Range)))
			return false;
	}

	Thread.Sleep(100 + Usefuls.Latency); /* ZZZzzzZZZzz */

	/* Target Reached */
	MovementManager.StopMove();
	MountTask.DismountMount();

	if (node.IsValid)
	{
		MovementManager.Face(node);
		Interact.InteractWith(node.GetBaseAddress);
	}
	else if (unit.IsValid)
	{
		MovementManager.Face(unit);
		Interact.InteractWith(unit.GetBaseAddress);
	}

	MovementManager.StopMove();
	MountTask.DismountMount();

	Lua.RunMacroText(questObjective.LuaMacro);

	Thread.Sleep(Usefuls.Latency + 150);

	if (node.IsValid)
	{
		nManagerSetting.AddBlackList(node.Guid, 30*1000);
	}
	else if (unit.IsValid)
	{
		nManagerSetting.AddBlackList(unit.Guid, 30*1000);
	}

	/* Wait if necessary */
	if (questObjective.WaitMs > 0)
		Thread.Sleep(questObjective.WaitMs);

	nManager.Wow.Helpers.Quest.GetSetIgnoreFight = false;
}
	/* Move to Zone/Hotspot */
else if (!MovementManager.InMovement)
{
	nManager.Wow.Helpers.Quest.GetSetIgnoreFight = false;
	if (questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)].DistanceTo(ObjectManager.Me.Position) > 5)
	{
		if(nManager.Wow.Helpers.Quest.TravelToQuestZone(questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)],
				ref questObjective.TravelToQuestZone, questObjective.ContinentId, questObjective.ForceTravelToQuestZone))
			return false;
		MovementManager.Go(PathFinder.FindPath(questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)]));
	}
	else
	{
		MovementManager.GoLoop(questObjective.PathHotspots);
	}
}
