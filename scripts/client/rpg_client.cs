// RPG Client

// Canvas.pushDialog(someDlg);

$CLIENT_EXEC_PATH = "scripts/client/";
function client_exec(%script)
{
	exec($CLIENT_EXEC_PATH @ %script @ ".cs");
}

// Dedicated RPG Binds
//GlobalActionMap.bind(keyboard, "tab", clientToggleTabMenu);
//GlobalActionMap.bind(keyboard, "i", clientToggleInventory);
GlobalActionMap.bind(keyboard, "lcontrol", toggleControl);
GlobalActionMap.bind(keyboard, "lshift", toggleShift);
GlobalActionMap.bind(keyboard, "lalt", toggleAlt);

$RPGClientEchoInventory = true;
function clientecho(%script, %func, %arg_0, %arg_1, %arg_2, %arg_3, %arg_4, %arg_5, %arg_6, %arg_7)
{
	if (%script != true)
		return;
	%new_str = "\c4" @ %func @ "->\c0";
	%arg_list = "";
	for (%i = 0; %i <= 7; %i++) {
		if (%arg_[%i] !$= "")
			%arg_list = %arg_list @ %arg_[%i] @ ">";
	}
	%new_str = %new_str @ %arg_list;
	echo(%new_str);
}

$RPGClientNull = "RPG_NULL";
%i = 0;
// Inventory
$RPGClientUseTextInventory = %i++;
$RPGClientTextInventoryGui = %i++;
$RPGClientInventoryGui = %i++;
$RPGClientMouseOutsideGui = %i++;
$RPGClientPickingItemGui = %i++;
$RPGClientMouseEnter = %i++;
$RPGClientItemInfoGui = %i++;
$RPGClientItemInfoText = %i++;
$RPGClientInventorySlotGui = %i++;
$RPGClientInventoryGuiSlot = %i++;
$RPGClientInitInventory = %i++;
$RPGClientInventoryIsSlot = %i++;
$RPGClientInventoryCount = %i++;
$RPGClientInventoryText = %i++;
$RPGClientPickingItem = %i++;
$RPGClientPickingWait = %i++;
$RPGClientPickingItemIcon = %i++;
$RPGClientPickingPosition = %i++;
$RPGClientSlotFound = %i++;
$RPGClientShowInventory = %i++;
$RPGClientInventoryItem = %i++;
$RPGClientInventoryIcon = %i++;
$RPGClientInventoryInfo = %i++;
$RPGClientEquipItem = %i++;
$RPGClientEquipIcon = %i++;
$RPGClientEquipInfo = %i++;
$RPGClientEquipSlotGui = %i++;
$RPGClientEquipGuiSlot = %i++;
$RPGClientWearGuiSlot = %i++;
$RPGClientWearItem = %i++;
$RPGClientWearInfo = %i++;
$RPGClientWearText = %i++;
$RPGClientLastMouse = %i++;
$RPGClientMouseOutside = %i++;
$RPGClientMouseInventory = %i++;
$RPGClientMouseWear = %i++;
$RPGClientMouseBank = %i++;
$RPGClientPickType = %i++;
$RPGClientTypeInventory = %i++;
$RPGClientTypeWear = %i++;
$RPGClientTypeEquip = %i++;
$RPGClientHoldShift = %i++;
$RPGClientHoldControl = %i++;
$RPGClientHoldAlt = %i++;
$RPGClientShowTabMenu = %i++;
// Tab Menu
$RPGClientTabMenuGui = %i++;
$RPGClientShowTabMenu = %i++;

function setClientData(%data, %arg_0, %arg_1)
{
	if (%arg_1 !$= "")
		$rpg_client_data[%data, %arg_0] = %arg_1;
	else
		$rpg_client_data[%data] = %arg_0;
}

function getClientData(%data, %arg_0)
{
	if (%arg_0 !$= "")
		return $rpg_client_data[%data, %arg_0];
	else
		return $rpg_client_data[%data];
}

function GuiMouseEventCtrl::onMouseMove(%this, %modifier, %mousePoint, %mouseClickCount)
{
	if (getClientData($RPGClientPickingItem) !$= $RPGNull)
		SelectedItem.setPosition(getWord(%mousePoint, 0) + 16, getWord(%mousePoint, 1) - 0);
}

function GuiMouseEventCtrl::onMouseEnter(%this, %modifier, %mousePoint, %mouseClickCount)
{
	%name = %this.name;
	if (getClientData($RPGClientShowInventory)) {
		RPGClientInventory::MouseEnter(%name, %mousePoint);
	}
}

function GuiMouseEventCtrl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
	%name = %this.name;
	RPGClientInventory::MouseDown(%name, %mousePoint);
}

setClientData($RPGClientHoldShift, false);
function toggleShift(%val) 
{
	if (%val)
		setClientData($RPGClientHoldShift, true);
	else
		setClientData($RPGClientHoldShift, false);
}

setClientData($RPGClientHoldControl, false);
function toggleControl(%val) 
{
	if (%val)
		setClientData($RPGClientHoldControl, true);
	else
		setClientData($RPGClientHoldControl, false);
}

setClientData($RPGClientHoldAlt, false);
function toggleAlt(%val)
{
	if (%val)
		setClientData($RPGClientHoldAlt, true);
	else
		setClientData($RPGClientHoldAlt, false);	
}

function jet(%val)
{
	$mvTriggerCount1++;
}

echo("\c4RPG_Client->\c0Loading Client RPG Scripts...");
exec("./rpg_client_inventory.cs");
exec("./rpg_client_menu.cs");


echo("\c4RPG_Client->\c0RPG Client Load Success.");