// RPG INVENTORY

// Maximum Inventory Space for currently selected "tab" of inventory. Bank can have multiple tabs.
// This is defined by the UI, if more slots are wanted, will have to add more slots into the Play GUI.
$RPG_MAX_INVENTORY = setRPGConst("max_inventory", 64);

function serverCmdInventoryRequest(%client)
{
	RPGInventory::Request(%client, 0);
}

function serverCmdInventoryPickItem(%client, %slot)
{
	RPGInventory::PickItem(%client, %slot);
}

function serverCmdInventoryDropPickingItem(%client, %slot)
{
	RPGInventory::DropPickingItem(%client, %slot);
}

function serverCmdAutoItem(%client, %slot, %equip)
{
	RPGInventory::AutoItem(%client, %slot, %equip);
}

function verifySlot(%slot)
{
	if (%slot < 0) %slot = 0;
	if (%slot > getRPGData($RPG_MAX_INVENTORY)) %slot = 0;
	return %slot;
}

function RPGInventory::AutoItem(%client, %slot, %equip)
{
	rpgecho($RPGEchoInventory, "RPGInventory::AutoItem", %client, %slot, %equip);
	if (!%equip) {
		%item = getPlayerInventory(%client, 0, %slot);
		%item_type = getRPGData(%item, $RPGDataItemType);
		switch (%item_type) {
			case $RPGDataItemTypeWearable:
				RPGWear::AutoEquip(%client, %item, %slot);
			case $RPGDataItemTypeConsumable:
				return;
		}
	}
	else
		RPGWear::AutoUnequip(%client, %slot);
}

function RPGInventory::DropPickingItem(%client, %slot)
{
	rpgecho($RPGEchoInventory, "RPGInventory::DropPickingItem", %client, %slot);
	%slot = verifySlot(%slot);
	%picking_item = getPlayerData(%client, $RPGPlayerPickingItem);
	%picking_count = getPlayerData(%client, $RPGPlayerPickingCount);
	if (%picking_item $= $RPGNull)
		return;
	%drop_slot = getPlayerInventory(%client, 0, %slot);
	if (%drop_slot $= $RPGNull) {
		// The drop slot is empty, fill the slot with the picking item
		setPlayerInventory(%client, 0, %slot, %picking_item, %picking_count);
		%info = $RPGNull;
		commandToClient(%client, 'ClearPickingItem');
		RPGInventory::ClientAddInventory(%client, %slot, %picking_item, %picking_count, false);
		setPlayerData(%client, $RPGPlayerPickingItem, $RPGNull);
	}
	else {
		if (%drop_slot $= %picking_item) {
			// The drop slot item is the same as the picking item, stack the items
			%current_count = getPlayerInventory(%client, 0, %slot, true);
			%new_count = %picking_count + %current_count;
			commandToClient(%client, 'ClearPickingItem');
			RPGInventory::ClientAddInventory(%client, %slot, %picking_item, %new_count, false);
			setPlayerData(%client, $RPGPlayerPickingItem, $RPGNull);
			setPlayerInventory(%client, 0, %slot, %picking_item, %new_count);
			RPG::Msg(%client, "You stack " @ %picking_count @ " " @ getRPGData(%picking_item, $RPGDataText) @ ".");
		}
	}
}

function RPGInventory::PickItem(%client, %slot)
{
	rpgecho($RPGEchoInventory, "RPGInventory::PickItem", %client, %slot);
	if (getPlayerData(%client, $RPGPlayerPickingItem) !$= $RPGNull)
		return;
	%item = getPlayerInventory(%client, 0, %slot);
	if (%item !$= $RPGNull && %item !$= "0") {
		%count = getPlayerInventory(%client, 0, %slot, true);
		setPlayerData(%client, $RPGPlayerPickingItem, %item);
		setPlayerData(%client, $RPGPlayerPickingCount, %count);
		commandToClient(%client, 'PickInventory', %slot);
		clearPlayerInventorySlot(%client, 0, %slot, true);
	}
}

function RPGInventory::ClientAddInventory(%client, %slot, %item, %count, %equip)
{
	rpgecho($RPGEchoInventory, "RPGInventory::ClientAddInventory", %slot, %item, %equip);
	%info = RPGItem::GetInfo(%item);
	%text = RPGItem::GetText(%item);
	%info_0 = getSubStr(%info, 0, 256);
	%info_1 = getSubStr(%info, 255, 256);
	%info_2 = getSubStr(%info, 511, 256);
	%info_3 = getSubStr(%info, 767, 256);
	commandToClient(%client, 'AddInventory', %slot, %item, %text, %count, %equip, %info_0, %info_1, %info_2, %info_3);
}

function RPGInventory::Request(%client, %inventory)
{
	rpgecho($RPGEchoInventory, "RPGInventory::Request", %client, %inventory);
	RPGWear::Request(%client);
	for (%i = 0; %i <= getRPGData($RPG_MAX_INVENTORY); %i++) {
		%item = getPlayerInventory(%client, %inventory, %i, false);
		%count = getPlayerInventory(%client, %inventory, %i, true);
		if (%item !$= $RPGNull) {
			RPGInventory::ClientAddInventory(%client, %i, %item, %count, false);
		}
	}
}

function RPGInventory::DefaultInventory(%client)
{
	rpgecho($RPGEchoInventory, "RPGInventory::DefaultInventory", %client);
	for (%i = 0; %i <= 7; %i++) {
		for (%x = 0; %x <= getRPGData($RPG_MAX_INVENTORY); %x++)
			setPlayerInventory(%client, %i, %x, $RPGNull);
	}
}

function RPGInventory::Add(%client, %inventory, %item, %update)
{
	rpgecho($RPGEchoInventory, "RPGInventory::Add", %client, %item);
	%free_slot = findPlayerInventory(%client, %inventory, %item);
	%slot = getWord(%free_slot, 0);
	%count = getWord(%free_slot, 1);
	if (%free_slot != -1) {
		%count += 1;
		setPlayerInventory(%client, %inventory, %slot, %item, %count);
		// Send the Update to the Client
		if (%update) {
			%icon = getRPGData(%item, $RPGDataItemIcon);
			RPGInventory::ClientAddInventory(%client, %free_slot, %item, false);
		}
		return true;
	}
	return false;
}

function findPlayerInventory(%client, %inventory, %item)
{
	rpgecho($RPGEchoInventory, "findPlayerInventory", %client, %inventory, %item);
	%found = false;
	%stack_slot = -1;
	%stack_count = 0;
	%x = 0;
	while (!%found) {
		%slot_item = getPlayerInventory(%client, %inventory, %x, false);
		%slot_count = getPlayerInventory(%client, %inventory, %x, true);
		if (%slot_item $= %item) {
			%found = true;
			%stack_slot = %x;
			break;
		}
		if (%slot_item $= $RPGNull) {
			if (%stack_slot == -1) {
				%stack_slot = %x;
				break;
			}
		}
		%x += 1;
	}
	if (%x >= getRPGData($RPG_MAX_INVENTORY) && !%found)
		%found = true;
	return %stack_slot @ " " @ %slot_count;
}

function getPlayerInventory(%client, %inventory, %slot, %count)
{
	if ($rpg_player_inventory[%client, %inventory, %slot, $RPGDataItem] $= "")
		return $RPGNull;
	if (!%count)
		return $rpg_player_inventory[%client, %inventory, %slot, $RPGDataItem];
	else
		return $rpg_player_inventory[%client, %inventory, %slot, $RPGDataItemCount];
}

function setPlayerInventory(%client, %inventory, %slot, %item, %count)
{
	if (%slot < 0 || %slot > getRPGData($RPG_MAX_INVENTORY))
		%slot = 0;
	if (%count $= "")
		$rpg_player_inventory[%client, %inventory, %slot, $RPGDataItem] = %item;
	else {
		$rpg_player_inventory[%client, %inventory, %slot, $RPGDataItem] = %item;
		$rpg_player_inventory[%client, %inventory, %slot, $RPGDataItemCount] = %count;
	}
}

function clearPlayerInventorySlot(%client, %inventory, %slot, %update)
{
	setPlayerInventory(%client, %inventory, %slot, $RPGNull, 0);
	if (%update)
		commandToClient(%client, 'ClearInventorySlot', %slot);
}

echo("\c1RPG->\c0Inventory Loaded.");