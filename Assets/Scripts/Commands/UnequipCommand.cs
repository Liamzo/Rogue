using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnequipCommand : Command
{
	BaseEquipment equipment;

    public UnequipCommand (UnitController owner, BaseEquipment equipment) : base(owner)
    {
        this.equipment = equipment;
    }

	public override CommandResult perform()
	{
        owner.equipmentManager.Unequip((int)equipment.item.equipSlot);

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
