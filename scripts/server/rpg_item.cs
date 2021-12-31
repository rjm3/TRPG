// RPG Item

// Define Item Classes:
function RPGItem::AddClass(%class_name, %text)
{
	setRPGData(%class_name, $RPGDataText, %text);
	return %class_name;
}
$ItemClassSword = RPGItem::AddClass("item_class_sword", "Sword");
$ItemClassBodyArmor = RPGItem::AddClass("item_class_body_armor", "Body Armor");

//================================================================
// Item Definitions:
function RPGItem::BaseItem(%item, %item_class, %text)
{
	setRPGData(%item, $RPGDataItemClass, %item_class);
	setRPGData(%item, $RPGDataText, %text);
	setRPGData(%item, $RPGDataItemType, $RPGDataItemTypeWearable);
	return %item;
}

// Broad Sword
$BroadSword = RPGItem::BaseItem("item_broad_sword", $ItemClassSword, "Broad Sword");
setRPGData($BroadSword, $RPGDataItemShape, "shape_broad_sword");
setRPGData($BroadSword, $RPGDataItemIcon, "icon_broad_sword");
setRPGData($BroadSword, $RPGDataItemBaseModifiers, "ATK:4");
setRPGData($BroadSword, $RPGDataItemRequirements, "LVL:1,SLA:1");
setRPGData($BroadSword, $RPGDataItemDesc, "A rusted broken blade, dull to the touch");
setRPGData($BroadSword, $RPGDataWearSlot, $WearWeapon);

// Padded Armor
$PaddedArmor = RPGItem::BaseItem("item_padded_armor", $ItemClassBodyArmor, "Padded Armor");
setRPGData($PaddedArmor, $RPGDataItemSkin, "skin_padded");
setRPGData($PaddedArmor, $RPGDataItemIcon, "icon_padded_armor");
setRPGData($PaddedArmor, $RPGDataItemBaseModifiers, "DEF:16");
setRPGData($PaddedArmor, $RPGDataItemRequirements, "LVL:1,END:8");
setRPGData($PaddedArmor, $RPGDataItemDesc, "Padded armor made from flimsy materials, hardly any protection");
setRPGData($PaddedArmor, $RPGDataWearSlot, $WearBodyArmor);

//================================================================

$INFO_COLOR_GREY = "<color:6a6a6a>";
$INFO_COLOR_WHITE = "<color:dbdbdb>";
$RPG_COLOR_EMPTY_WEAR = "<color:6f5142>";

function RPGItem::GetText(%item)
{
	return $INFO_COLOR_WHITE @ getRPGData(%item, $RPGDataText);
}

function RPGItem::GetInfo(%item)
{
	rpgecho($RPGEchoItem, "RPGItem::GetInfo", %item);
	%info = "Unknown Item";
	%type = getRPGData(%item, $RPGDataItemType);
	switch$ (%type) {
		case $RPGDataItemTypeWearable:
			%info = RPGItem::GetWearableInfo(%item);
	}
	return %info;
}

function RPGItem::Create(%item_type, %item_level)
{
	%item = RPGItem::New(%item_type, %item_level);
	RPGItem::Build(%item);
	return %item;
}

function RPGItem::New(%item_type, %item_level)
{
	rpgecho($RPGEchoItem, "RPGItem::New", %item_type, %item_level);
	%build = "";
	%build = %item_type @ "^" @ %item_level @ "^";
	%base_modifiers = $rpg_data[%item_type, $RPGDataItemBaseModifiers];
	%build = %build @ %base_modifiers @ "^";
	%requirements = $rpg_data[%item_type, $RPGDataItemRequirements];
	if (%requirements $= "")
		%requirements = "LVL:0";
	%build = %build @ %requirements;
	return %build;
}

function RPGItem::Build(%item)
{
	rpgecho($RPGEchoItem, "RPGItem::Build", %item);
	%str = %item;
	%i = 0;
	while (%str !$= "") {
		%str = nextToken(%str, "token", "^");
		%break_data[%i] = %token;
		%i += 1;
	}
	%item_type = %break_data[0];
	%item_level = %break_data[1];
	%item_base = %break_data[2];
	%item_requirements = %break_data[3];
	%text = getRPGData(%item_type, $RPGDataText);
	setRPGData(%item, $RPGDataText, %text);
	%item_class = getRPGData(%item_type, $RPGDataItemClass);
	setRPGData(%item, $RPGDataItemClass, %item_class);
	setRPGData(%item, $RPGDataItemLevel, %item_level);
	setRPGData(%item, $RPGDataItemBaseModifiers, %item_base);
	setRPGData(%item, $RPGDataItemRequirements, %item_requirements);
	%desc = getRPGData(%item_type, $RPGDataItemDesc);
	setRPGData(%item, $RPGDataItemDesc, %desc);
	%slot = getRPGData(%item_type, $RPGDataWearSlot);
	setRPGData(%item, $RPGDataWearSlot, %slot);
	%icon = getRPGData(%item_type, $RPGDataItemIcon);
	setRPGData(%item, $RPGDataItemIcon, %icon);
	%item_type = getRPGData(%item_type, $RPGDataItemType);
	setRPGData(%item, $RPGDataItemType, %item_type);
}

function RPGItem::BreakBonus(%arg_0)
{
	%str = %arg_0;
	%i = 0;
	while (%str !$= "") {
		%str = nextToken(%str, "token", ":");
		%break_data[%i] = %token;
		%i += 1;
	}
	%arg_1 = %break_data[0];
	%arg_2 = %break_data[1];
	return %arg_1 @ " " @ %arg_2;
}

function RPGItem::GetWearableInfo(%item)
{
	rpgecho($RPGEchoItem, "RPGItem::GetWearableInfo", %item);
	%info = "";
	%item_name = getRPGData(%item, $RPGDataText);
	%item_level = getRPGData(%item, $RPGDataItemLevel);
	%item_class = getRPGData(%item, $RPGDataItemClass);
	%item_class = getRPGData(%item_class, $RPGDataText);
	%desc = getRPGData(%item, $RPGDataItemDesc);
	// Header
	%info = %info @ %item_name @ "\n";
	%info = %info @ $INFO_COLOR_GREY @ %item_class @ "\n\n";
	// Requirements
	%info = %info @ $INFO_COLOR_GREY @ "Item Level: " @ $INFO_COLOR_WHITE @ %item_level @ "\n";
	%requirements = getRPGData(%item, $RPGDataItemRequirements);
	%str = %requirements;
	%i = 0;
	while (%str !$= "") {
		%str = nextToken(%str, "token", ",");		
		%break_data[%i] = %token;
		%i += 1;
	}
	for (%x = 0; %x < %i; %x++) {
		%req = RPGItem::BreakBonus(%break_data[%x]);
		%req_type = getWord(%req, 0);
		%req_value = getWord(%req, 1);
		if (%req !$= "") {
			%decode = getDecode(%req_type);
			%info = %info @ $INFO_COLOR_GREY @ "Required " @ getRPGData(%decode, $RPGDataText) @ ": " @ $INFO_COLOR_WHITE @ %req_value @ "\n";
		}
	}
	%info = %info @ "\n";
	// Bonus
	%bonus_text = "";
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
			%bonus_text = %bonus_text @ "\t" @ getRPGData(%decode, $RPGDataText) @ " " @ %bonus_value @ "\n";
		}
	}
	%info = %info @ $INFO_COLOR_WHITE @ trim(%bonus_text) @ "\n\n";
	%info = %info @ $INFO_COLOR_GREY @ %desc;
	return %info;
}

// Test some Items
%item = RPGItem::New($PaddedArmor, 4);
RPGItem::Build(%item);
//RPGItem::ShowInfo(%item);
//%item = RPGItem::New($BroadSword, 8);
//RPGItem::Build(%item);
//RPGItem::ShowInfo(%item);

echo("\c1RPG->\c0Item Loaded.");