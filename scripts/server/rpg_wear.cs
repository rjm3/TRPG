// RPG Wear

function RPGWear::Add(%wear_name, %text)
{
	setRPGData(%wear_name, $RPGDataText, %text);
	setRPGData(%wear_name, $RPGDataType, $RPGDataTypeInventory);
	return %wear_name;
}

$WearOrb = RPGWear::Add("wear_orb", "Orb");
$WearHelmet = RPGWear::Add("wear_helmet", "Helmet");
$WearAmulet = RPGWear::Add("wear_amulet", "Amulet");
$WearWeapon = RPGWear::Add("wear_weapon", "Weapon");
$WearBodyArmor = RPGWear::Add("wear_body_armor", "Body Armor");
$WearOffHand = RPGWear::Add("wear_off_hand", "Off Hand");
$WearGloves = RPGWear::Add("wear_gloves", "Gloves");
$WearBelt = RPGWear::Add("wear_belt", "Belt");
$WearBoots = RPGWear::Add("wear_boots", "Boots");
$WearRing = RPGWear::Add("wear_ring", "Ring");
$WearArtifact = RPGWear::Add("wear_artifact", "Artifact");
$WearTome = RPGWear::Add("wear_tome", "Tome");
$WearTalisman = RPGWear::Add("wear_talisman", "Talisman");

function serverCmdRequestEquipWear(%client, %slot)
{
	RPGWear::RequestEquipWear(%client, %slot);
}

function serverCmdWearPickItem(%client, %slot)
{
	RPGWear::PickItem(%client, %slot);
}

function RPGWear::PickItem(%client, %slot)
{
	rpgecho($RPGEchoWear, "RPGWear::PickItem", %client, %slot);
	%item = fetchData(%client, toArray($RPGArrayPlayerWear, %slot));
	if (%item !$= $RPGNull) {
		unWearItem(%client, %slot);
		setPlayerData(%client, $RPGPlayerPickingItem, %item);
		setPlayerData(%client, $RPGPlayerPickingCount, 1);
		%wear_slot = getRPGData($RPGDataWear, toArray($RPGArrayWearSlot, %slot));
		%empty_text = $RPG_COLOR_EMPTY_WEAR @ getRPGData(%wear_slot, $RPGDataText);
		commandToClient(%client, 'PickWear', %slot, %empty_text);
		RPG::Msg(%client, "You remove " @ getRPGData(%item, $RPGDataText) @ ".");
	}
}

function RPGWear::AutoUnWear(%client, %slot)
{
	rpgecho($RPGEchoWear, "RPGWear::AutoUnWear", %client, %slot);
	return;
}

function RPGWear::AutoWear(%client, %item, %inventory_slot)
{
	rpgecho($RPGEchoWear, "RPGWear::AutoWear", %client, %item);
	return;
}

function RPGWear::ClientSetWear(%client, %slot, %item, %clear_picking)
{
	%text = RPGItem::GetText(%item);
	%info = RPGItem::GetInfo(%item);
	%info_0 = getSubStr(%info, 0, 256);
	%info_1 = getSubStr(%info, 256, 256);
	%info_2 = getSubStr(%info, 512, 256);
	%info_3 = getSubStr(%info, 768, 256);
	commandToClient(%client, 'SetWear', %slot, %item, %text, %clear_picking, %info_0, %info_1, %info_2, %info_3);
}

function RPGWear::RequestEquipWear(%client, %slot)
{
	rpgecho($RPGEchoWear, "RPGWear::RequestEquipWear", %client, %slot);
	%picking_item = fetchData(%client, $RPGPlayerPickingItem);
	if (%picking_item $= $RPGNull)
		return;
	if ((%find_slot = findWearSlot(%client, %picking_item)) != -1) {
		echo(" found slot? " @ %find_slot);
		// A matching slot has been found, try to equip
		%current = fetchData(%client, toArray($RPGArrayPlayerWear, %find_slot));
		%wear_name = getRPGData(getRPGData(%current, $RPGDataWearSlot), $RPGDataText);
		if (%current $= $RPGNull) {
			// Not wearing anything in the matching slot, lets equip
			if (wearRequirements(%client, %picking_item)) {
				%item_name = getRPGData(%picking_item, $RPGDataText);
				%picking_count = fetchData(%client, $RPGPlayerPickingCount);
				%picking_count -= 1;
				if (%picking_count > 0) {
					storeData(%client, $RPGPlayerPickingCount, %picking_count);
					storeData(%client, $RPGPlayerPickingItem, %picking_item);
					%item_text = RPGItem::GetText(%picking_item);
					commandToClient(%client, 'UpdatePicking', %item_text, %picking_count);
					RPGWear::ClientSetWear(%client, %find_slot, %picking_item, false);
				}
				else {
					storeData(%client, $RPGPlayerPickingItem, $RPGNull);
					RPGWear::ClientSetWear(%client, %find_slot, %picking_item, true);
				}
				wearItem(%client, %picking_item, %find_slot);
				RPG::Msg(%client, "You wield " @ %item_name @ ".");
			}
			else
				RPGMsg(%client, "You don't meet the requirements to wear this item.");
		}
		else
			RPG::Msg(%client, "Item already worn in slot: " @ %wear_name);
	}
}

function findWearSlot(%client, %item)
{
	rpgecho($RPGEchoWear, "findWearSlot", %client, %item);
	%item_wear_slot = getRPGData(%item, $RPGDataWearSlot);
	%wear_count = getRPGData($RPGDataWear, $RPGDataWearCount);
	%found_slot = -1;
	%i = 0;
	while (%i <= %wear_count) {
		%wear_slot = getRPGData($RPGDataWear, toArray($RPGArrayWearSlot, %i));
		if (%wear_slot $= %item_wear_slot) {
			%found_slot = %i;
			%found = true;
			break;
		}
		%i += 1;
		if (%i > (%wear_count - 1))
			break;
	}
	return %found_slot;
}

function wearRequirements(%client, %item)
{
	%str = getRPGData(%item, $RPGDataItemRequirements);
	%i = 0;
	%ok = true;
	while (%str !$= "") {
		%str = nextToken(%str, "token", ",");		
		%break_data[%i] = %token;
		%i += 1;
	}
	for (%x = 0; %x < %i; %x++) {
		%requirement = %break_data[%x];
		%require_type = getWord(%requirement, 0);
		%require_value = getWord(%requirement, 1);
		if (%requirement !$= "") {
			%decode = getDecode(%require_type);
			%player_val = fetchData(%client, %decode, true);
			if (%player_val < %require_value) {
				wearDenyMsg(%client, getRPGData(%decode, $RPGDataText), %require_value);
				%ok = false;
			}
		}
	}
	return %ok;
}

function wearItem(%client, %item, %slot)
{
	storeData(%client, toArray($RPGArrayPlayerWear, %slot), %item);
	RPGWear::Bonus(%client, %item, false);
}

function unWearItem(%client, %slot)
{
	%item = fetchData(%client, toArray($RPGArrayPlayerWear, %slot));
	storeData(%client, toArray($RPGArrayPlayerWear, %slot), $RPGNull);
	RPGWear::Bonus(%client, %item, true);
}

function wearDenyMsg(%client, %type, %need)
{
	RPG::Msg(%client, %type @ " must be at least " @ %need @ ".");
}

function RPGWear::Bonus(%client, %item, %unwear)
{
	rpgecho($RPGEchoWear, "RPGWear::Bonus", %client, %item, %unwear);
	%base_modifiers = getRPGData(%item, $RPGDataItemBaseModifiers);
	%str = %base_modifiers;
	%i = 0;
	while (%str !$= "") {
		%str = nextToken(%str, "token", ",");		
		%break_data[%i] = %token;
		%i += 1;
	}
	for (%x = 0; %x < %i; %x++) {
		%bonus = RPGItem::BreakBonus(%break_data[%x]);
		%bonus_type = getWord(%bonus, 0);
		%bonus_value = getWord(%bonus, 1);
		if (%bonus !$= "") {
			%decode = getDecode(%bonus_type);
			if (%unwear)
				%bonus_value *= -1;
			addBonus(%client, %decode, %bonus_value, true);
		}
	}
}

function RPGWear::Init()
{
	setRPGData($RPGDataWear, $RPGDataWearCount, 0);
	setRPGData($RPGDataWear, $RPGDataWearMasterList, "");
}
RPGWear::Init();

function RPGWear::AddWearSlot(%wear)
{
	%wear_count = getRPGData($RPGDataWear, $RPGDataWearCount);
	%wear_list = getRPGData($RPGDataWear, $RPGDataWearMasterList);
	if (%wear_list $= $RPGNull) 
		%wear_list = "";
	setRPGData($RPGDataWear, toArray($RPGArrayWearSlot, %wear_count), %wear);
	setRPGData($RPGDataWear, $RPGDataWearCount, %wear_count + 1);
	%wear_list = %wear_list @ %wear @ " ";
	setRPGData($RPGDataWear, $RPGDataWearMasterList, %wear_list);
}
RPGWear::AddWearSlot($WearAmulet);
RPGWear::AddWearSlot($WearOrb);
RPGWear::AddWearSlot($WearHelmet);
RPGWear::AddWearSlot($WearBodyArmor);
RPGWear::AddWearSlot($WearWeapon);
RPGWear::AddWearSlot($WearOffHand);
RPGWear::AddWearSlot($WearBelt);
RPGWear::AddWearSlot($WearGloves);
RPGWear::AddWearSlot($WearRing);
RPGWear::AddWearSlot($WearRing);
RPGWear::AddWearSlot($WearBoots);
RPGWear::AddWearSlot($WearArtifact);

function RPGWear::Request(%client)
{
	rpgecho($RPGEchoWear, "RPGWear::Request", %client);
	%wear_count = getRPGData($RPGDataWear, $RPGDataWearCount);
	for (%i = 0; %i <= %wear_count; %i++) {
		%current = fetchData(%client, toArray($RPGArrayPlayerWear, %i));
		// Player has nothing equipped in the area:
		if (%current $= $RPGNull) {
			%wear_slot = getRPGData($RPGDataWear, toArray($RPGArrayWearSlot, %i));
			if (%wear_slot !$= $RPGNull) {
				%wear_text = $RPG_COLOR_EMPTY_WEAR @ "<just:center>" @ getRPGData(%wear_slot, $RPGDataText);
				commandToClient(%client, 'SetWear', %i, $RPGNull, %wear_text);
			}
		}
		// Player has something equipped:
		else {
			// Get wear item info here
		}
	}
}

function RPGWear::DefaultWear(%client)
{
	rpgecho($RPGEchoWear, "RPGWear::DefaultWear", %client);
	%wear_count = getRPGData($RPGDataWear, $RPGDataWearCount);
	for (%i = 0; %i <= %wear_count; %i++)
		storeData(%client, toArray($RPGArrayPlayerWear, %i), $RPGNull);
}

echo("\c1RPG->\c0Wear Loaded.");