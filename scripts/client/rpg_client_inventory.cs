// RPG Client Inventory

setClientData($RPGClientTextInventoryGui, "TextInventory");
setClientData($RPGClientMouseOutsideGui, "mouse_outside");
setClientData($RPGClientPickingItemGui, "SelectedItem");
setClientData($RPGClientPickingItemText, "selected_item_text");
setClientData($RPGClientItemInfoGui, "ItemInfo");
setClientData($RPGClientItemInfoText, "ItemInfoText");
setClientData($RPGClientPickingItem, $RPGClientNull);
setClientData($RPGClientPickingWait, false);
setClientData($RPGClientPickingPosition, $RPGClientNull);

function setClientWearData(%slot, %data, %arg_0)
{
	$rpg_client_wear[%slot, %data] = %arg_0;
}

function getClientWearData(%slot, %data)
{
	%return_data = $rpg_client_wear[%slot, %data];
	if (%return_data $= "")
		return $RPGClientNull;
	return %return_data;
}

function setClientInventoryData(%slot, %data, %arg_0, %arg_1)
{
	$rpg_client_inventory[%slot, %data] = %arg_0;
}

function getClientInventoryData(%slot, %data)
{
	%return_data = $rpg_client_inventory[%slot, %data];
	if (%return_data $= "")
		return $RPGClientNull;
	return %return_data;
}

function RPGClientInventory::Default()
{
	%mouse_outside = getClientData($RPGClientMouseOutsideGui);
	%inventory = getClientData($RPGClientTextInventoryGui);
	%selected_item = getClientData($RPGClientPickingItemGui);
	%item_info = getClientData($RPGClientItemInfoGui);
	%mouse_outside.setVisible(false);
	%inventory.setVisible(false);
	%selected_item.setVisible(false);
	%item_info.setVisible(false);
	setClientData($RPGClientInitInventory, true);
	setClientData($RPGClientPickingItem, $RPGClientNull);
	setClientData($RPGClientShowInventory, false);
	setClientData($RPGClientMouseEnter, $RPGClientNull);
	for (%i = 0; %i <= 63; %i++) {
		%inventory_text = "inventory_text_" @ %i;
		%t_inventory = "t_inventory_" @ %i;
		setClientInventoryData(%i, $RPGClientInventoryItem, $RPGClientNull);
		setClientInventoryData(%i, $RPGClientInventoryInfo, $RPGClientNull);
		setClientInventoryData(%i, $RPGClientInventoryTextGui, %inventory_text);
		setClientInventoryData(%i, $RPGClientInventoryCount, 0);
		setClientInventoryData(%i, $RPGClientInventoryText, $RPGClientNull);
		setClientInventoryData(%i, $RPGClientInventorySlotGui, %t_inventory);
		setClientInventoryData(%t_inventory, $RPGClientInventoryGuiSlot, %i);
		setClientData(%t_inventory, $RPGClientPickType, $RPGClientTypeInventory);
		%inventory_text.setVisible(false);
	}
	for (%i = 0; %i <= 15; %i++) {
		%wear = "wear_" @ %i;
		%wear_text = "wear_text_" @ %i;
		%wear_text.setText("<just:center>(empty)");
		%wear_text.setVisible(false);
		setClientInventoryData(%wear, $RPGClientWearGuiSlot, %i);
		setClientData(%wear, $RPGClientPickType, $RPGClientTypeWear);
		%wear_text.setVisible(false);
	}
}
RPGClientInventory::Default();

function RPGClientInventory::Init() 
{
	setClientData($RPGClientInitInventory, false);
	commandToServer('InventoryRequest');
}

function RPGClientInventory::ClearInfo()
{
	%item_info_gui = getClientData($RPGClientItemInfoGui);
	if (%item_info_gui.visible)
		%item_info_gui.setVisible(false);
}

function RPGClientInventory::ShowInfo(%slot, %mouse_point, %wear)
{
	//clientecho($RPGClientEchoInventory, "RPGClientInventory::ShowInfo", %wear);
	if (%slot $= $RPGClientNull)
		return;
	if (getClientData($RPGClientPickingItem) !$= $RPGClientNull)
		return;
	if (!%wear) {
		%item = getClientInventoryData(%slot, $RPGClientInventoryItem);
		%info = getClientInventoryData(%slot, $RPGClientInventoryInfo);
		%info = %info @ "\n\n<color:1b1e22>CTRL + Click to Wear item\nSHIFT + Click to Move item";
	}
	else {
		%item = getClientWearData(%slot, $RPGClientWearItem);
		%info = getClientWearData(%slot, $RPGClientWearInfo);
	}
	if (%item $= $RPGClientNull) {
		RPGClientInventory::ClearInfo();
		return;
	}
	%mouse_x = getWord(%mouse_point, 0);
	%mouse_y = getWord(%mouse_point, 1);
	%item_info_gui = getClientData($RPGClientItemInfoGui);
	%item_info_text = getClientData($RPGClientItemInfoText);
	%item_info_text.setText("<just:center>" @ %info);
	%item_info_gui.setVisible(true);
	%item_info_text.forceReflow();
	%extent = %item_info_text.getExtent();
	%extent_x = getWord(%extent, 0);
	%extent_y = getWord(%extent, 1);
	%item_info_gui.setExtent(%extent_x, %extent_y);
	%item_info_gui.setPosition(%mouse_x + 64, %mouse_y + 32);
}

function RPGClientInventory::MouseEnter(%name, %mouse_point)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::MouseEnter", %name, %mouse_point);
	if (%name $= "mouse_outside") {
		setClientData($RPGClientLastMouse, $RPGClientMouseOutside);
		setClientData($RPGClientMouseEnter, -1);
		RPGClientInventory::ClearInfo();
	}
	if ((%slot = getClientInventoryData(%name, $RPGClientInventoryGuiSlot)) !$= $RPGClientNull) {
		setClientData($RPGClientMouseEnter, %slot);
		setClientData($RPGClientLastMouse, $RPGClientMouseInventory);
		if (%slot !$= "")
			RPGClientInventory::ShowInfo(%slot, %mouse_point, false);
		else
			RPGClientInventory::ClearInfo();
	}
	if ((%slot = getClientInventoryData(%name, $RPGClientWearGuiSlot)) !$= $RPGClientNull) {
		setClientData($RPGClientMouseEnter, %slot);
		setClientData($RPGClientLastMouse, $RPGClientMouseWear);
		if (%slot !$= $RPGClientNull)
			RPGClientInventory::ShowInfo(%slot, %mouse_point, true);
		else
			RPGClientInventory::ClearInfo();
	}
}

function RPGClientInventory::MouseDown(%name, %mouse_point)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::MouseDown", %name, %mouse_point);
	%picking_item = getClientData($RPGClientPickingItem);
	// No Item is getting Picked right now, try to Pick one up:
	if (%picking_item $= $RPGClientNull) {
		switch (getClientData(%name, $RPGClientPickType)) {
			case $RPGClientTypeInventory:
				%slot = getClientInventoryData(%name, $RPGClientInventoryGuiSlot);
				if (getClientInventoryData(%slot, $RPGClientInventoryItem) $= $RPGClientNull)
					return;
				if (getClientData($RPGClientPickingWait))
					return;
				setClientData($RPGClientPickingWait, true);
				commandToServer('InventoryPickItem', %slot, getClientData($RPGClientHoldControl));
				setClientData($RPGClientPickingPosition, %mouse_point);
			case $RPGClientTypeWear:
				%slot = getClientInventoryData(%name, $RPGClientWearGuiSlot);
				%item = getClientWearData(%slot, $RPGClientWearItem);
				if (%item !$= $RPGClientNull) {
					setClientData($RPGClientPickingWait, true);
					commandToServer('WearPickItem', %slot);
					setClientData($RPGClientPickingPosition, %mouse_point);
				}
		}
	}
	// Item is picked up, lets drop it:
	else {
		if ((%enter = getClientData($RPGClientMouseEnter)) !$= $RPGClientNull) {
			switch (getClientData($RPGClientLastMouse)) {
				case $RPGClientMouseInventory:
					commandToServer('InventoryDropPickingItem', %enter);
				case $RPGClientMouseWear:
					commandToServer('RequestEquipWear', %enter);
			}
		}
	}
}

function RPGClientInventory::Add(%slot, %item, %text, %count, %clear_picking, %info)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::Add", %slot, %item, %text, %count, %clear_picking);
	setClientInventoryData(%slot, $RPGClientInventoryItem, %item);
	setClientInventoryData(%slot, $RPGClientInventoryText, %text);
	setClientInventoryData(%slot, $RPGClientInventoryCount, %count);
	setClientInventoryData(%slot, $RPGClientInventoryInfo, %info);
	%text_gui = getClientInventoryData(%slot, $RPGClientInventoryTextGui);
	%text_gui.setText(%text @ "<just:right>" @ %count);
	%text_gui.setVisible(true);
}

function RPGClientInventory::PickItem(%slot)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::PickItem", %slot);
	setClientData($RPGClientPickingWait, false);
	setClientData($RPGClientPickingItem, %slot);
	%picking_gui = getClientData($RPGClientPickingItemGui);
	%picking_text = getClientData($RPGClientPickingText);
	%item_text = getClientInventoryData(%slot, $RPGClientInventoryText);
	%count = getClientInventoryData(%slot, $RPGClientInventoryCount);
	%picking_text.setText("<just:left>" @ %item_text @ " " @ %count);
	%mouse_point = getClientData($RPGClientPickingPosition);
	%picking_gui.setPosition(getWord(%mouse_point, 0) + 16, getWord(%mouse_point, 1) - 0);
	RPGClientInventory::ClearInfo();
	%picking_gui.setVisible(true);
}

function RPGClientInventory::UpdatePicking(%item_text, %count)
{
	%picking_gui = getClientData($RPGClientPickingItemGui);
	%picking_text = getClientData($RPGClientPickingText);
	%picking_text.setText("<just:left>" @ %item_text @ " " @ %count);
}

function RPGClientInventory::ClearSlot(%slot)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::ClearSlot", %slot);
	setClientInventoryData(%slot, $RPGClientInventoryItem, $RPGClientNull);
	setClientInventoryData(%slot, $RPGClientInventoryText, $RPGClientNull);
	setClientInventoryData(%slot, $RPGClientInventoryInfo, $RPGClientNull);
	setClientInventoryData(%slot, $RPGClientInventoryCount, 0);
	%inventory_text = "inventory_text_" @ %slot;
	%inventory_text.setVisible(false);
}

function RPGClientInventory::ClearPickingItem()
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::ClearPickingItem");
	setClientData($RPGClientPickingItem, $RPGClientNull);
	%picking_gui = getClientData($RPGClientPickingItemGui);
	%picking_gui.setVisible(false);
}

function RPGClientInventory::SetWear(%slot, %item, %text, %info)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::SetWear", %slot, %item, %text);
	%wear = "wear_" @ %slot;
	%wear_text = "wear_text_" @ %slot;
	%wear_text.setText("<just:center>" @ %text);
	%wear_text.setVisible(true);
	setClientWearData(%slot, $RPGClientWearItem, %item);
	setClientWearData(%slot, $RPGClientWearText, %text);
	setClientWearData(%slot, $RPGClientWearInfo, %info);
}

function RPGClientInventory::PickWear(%slot, %empty_text)
{
	clientecho($RPGClientEchoInventory, "RPGClientInventory::PickWear", %slot);
	setClientData($RPGClientPickingWait, false);
	setClientData($RPGClientPickingItem, %slot);
	%picking_gui = getClientData($RPGClientPickingItemGui);
	%picking_text = getClientData($RPGClientPickingText);
	%item_text = getClientWearData(%slot, $RPGClientWearText);
	%picking_text.setText("<just:left>" @ %item_text @ " 1");
	%mouse_point = getClientData($RPGClientPickingPosition);
	%picking_gui.setPosition(getWord(%mouse_point, 0) + 16, getWord(%mouse_point, 1) - 0);
	RPGClientInventory::ClearInfo();
	%picking_gui.setVisible(true);
	setClientWearData(%slot, $RPGClientWearItem, $RPGClientNull);
	setClientWearData(%slot, $RPGClientWearText, $RPGClientNull);
	setClientWearData(%slot, $RPGClientWearInfo, $RPGClientNull);
	%wear_text = "wear_text_" @ %slot;
	%wear_text.setText("<just:center>" @ %empty_text);
}

function clientCmdPickWear(%slot, %empty_text)
{
	RPGClientInventory::PickWear(%slot, %empty_text);
}

function clientCmdAddInventory(%slot, %item, %text, %count, %clear_picking, %info_0, %info_1, %info_2, %info_3)
{
	clientecho($RPGClientEchoInventory, "clientCmdAddInventory", %slot, %item, %text, %count, %clear_picking);
	%info = %info_0 @ %info_1 @ %info_2 @ %info_3;
	RPGClientInventory::Add(%slot, %item, %text, %count, %clear_picking, %info);
}

function clientCmdPickInventory(%slot)
{
	clientecho($RPGClientEchoInventory, "clientCmdPickInventory", %slot);
	RPGClientInventory::PickItem(%slot);
}

function clientCmdClearInventorySlot(%slot)
{
	RPGClientInventory::ClearSlot(%slot);
}

function clientCmdClearPickingItem()
{
	RPGClientInventory::ClearPickingItem();
}

function clientCmdUpdatePicking(%item_text, %count)
{
	RPGClientInventory::UpdatePicking(%item_text, %count);
}

function clientCmdSetWear(%slot, %item, %text, %clear_picking, %info_0, %info_1, %info_2, %info_3)
{
	%info = %info_0 @ %info_1 @ %info_2 @ %info_3;
	if (%clear_picking)
		RPGClientInventory::ClearPickingItem();
	RPGClientInventory::SetWear(%slot, %item, %text, %info);
}

function clientToggleInventory(%val)
{
	clientecho($RPGClientEchoInventory, "clientToggleInventory", %val);
	%mouse_outside_gui = getClientData($RPGClientMouseOutsideGui);
	%inventory_gui = getClientData($RPGClientTextInventoryGui);
	if (%val) {
		if (getClientData($RPGClientShowInventory)) {
			%inventory_gui.setVisible(false);
			%mouse_outside_gui.setVisible(false);
			setClientData($RPGClientShowInventory, false);
			RPGClientInventory::ClearInfo();
			hideCursor();
		}
		else {
			
			if (getClientData($RPGClientInitInventory))
				RPGClientInventory::Init();
			%inventory_gui.setVisible(true);
			%mouse_outside_gui.setVisible(true);
			setClientData($RPGClientShowInventory, true);
			showCursor();
		}
	}
}

echo("\c4RPG_Client->\c0Inventory Loaded");