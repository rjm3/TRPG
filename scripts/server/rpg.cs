// TORQUE 3D RPG
// NOTES:
// To get a Clients Object ID use %client.player
// To get an Objects Client use %object.client

$SERVER_EXEC_PATH = "scripts/server/";
function server_exec(%script)
{
	exec($SERVER_EXEC_PATH @ %script @ ".cs");
}

function rpgexec()
{
	exec("scripts/server/rpg.cs");
	exec("scripts/client/rpg_client.cs");
}

$RPGEchoItem = true;
$RPGEchoPlayer = true;
$RPGEchoInventory = true;
$RPGEchoLoot = true;
$RPGEchoWear = true;
$RPGEchoWeapon = true;
function rpgecho(%script, %func, %arg_0, %arg_1, %arg_2, %arg_3, %arg_4, %arg_5, %arg_6, %arg_7)
{
	if (%script != true)
		return;
	%new_str = "\c1" @ %func @ "->\c0";
	%arg_list = "";
	for (%i = 0; %i <= 7; %i++) {
		if (%arg_[%i] !$= "")
			%arg_list = %arg_list @ %arg_[%i] @ ">";
	}
	%new_str = %new_str @ %arg_list;
	echo(%new_str);
}

function RPG::Msg(%client, %msg)
{
	messageClient(%client, 0, %msg);
}

$RPGNull = "RPG_NULL";
%i = 0;
$RPGError = %i;
$RPGDataType = %i++;
// getRPGData Prefixes
$RPGPlayerData = %i++;
$RPGPlayerSkill = %i++;
$RPGPlayerBaseSkill = %i++;
$RPGPlayerBonusSkill = %i++;
$RPGPlayerData = %i++;
// Basic Data
$RPGDataConst = %i++;
$RPGDataText = %i++;
$RPGDataCode = %i++;
$RPGDataDecode = %i++;
$RPGDataBaseSkill = %i++;
$RPGDataBaseClass = %i++;
$RPGDataItemShape = %i++;
$RPGDataItemIcon = %i++;
$RPGDataItemSkin = %i++;
$RPGDataItemBaseModifiers = %i++;
$RPGDataItemClass = %i++;
$RPGDataItemRequirements = %i++;
$RPGDataItemLevel = %i++;
$RPGDataItemRarity = %i++;
$RPGDataItemDesc = %i++;
$RPGDataItemTypeWearable = %i++;
$RPGDataItemTypeConsumable = %i++;
$RPGDataItemType = %i++;
$RPGDataItem = %i++;
$RPGDataItemCount = %i++;
$RPGDataMaxInventory = %i++;
$RPGDataSlotToWear = %i++;
$RPGDataWear = %i++;
$RPGDataWearType = %i++;
$RPGDataWearSlot = %i++;
$RPGDataWearVar = %i++;
$RPGDataWearCount = %i++;
$RPGDataWearMasterList = %i++;
// Player
$RPGPlayerInventory = %i++;
$RPGPlayerPickingItem = %i++;
$RPGPlayerPickingCount = %i++;
$RPGPlayerWear = %i++;
// Data Types
$RPGDataTypeClient = %i++;
$RPGDataTypePlayer = %i++;
$RPGDataTypePlayerData = %i++;
$RPGDataTypeSkill = %i++;
$RPGDataTypeClass = %i++;
$RPGDataTypeClassData = %i++;
$RPGDataTypeItem = %i++;
$RPGDataTypeInventory = %i++;
$RPGDataTypeConst = %i++;
// Class Data
$RPGDataSuperClass = %i++;
$RPGDataClassSkillMultiplier = %i++;
$RPGDataClassLifeMultiplier = %i++;
$RPGDataClassManaMultiplier = %i++;
// Array Data
$RPGArrayWearSlot = "[wear_slot]";
$RPGArrayPlayerWear = "[player_wear]";

// Array helper function, all arrays are accessed via strings
function toArray(%array, %arg_0, %arg_1, %arg_2, %arg_3, %arg_4, %arg_5, %arg_6, %arg_7)
{
	return %array @ "" @ %arg_0 @ "" @ %arg_1 @ "" @ %arg_2 @ "" @ %arg_3 @ "" @ %arg_4 @ "" @ %arg_5 @ "" @ %arg_6 @ "" @ %arg_7;
}

//$RPGArray = %i++;
//function initRPGArray(%array, %n)
//{
//	for (%i = 0; %i <= %n; %i++) {
//		%array_name = %array @ "" @ %i;
//		$RPGArray[%array_name] = "";
//	}
//}
//function getRPGArrayData(%array, %x, %y, %z)
//{
//	%data = %array @ "" @ %x @ "" @ %y @ "" @ %z;
//	return $RPGArray[%data];
//}

$rpg_data = "";
$rpg_data[$RPGPlayerData, $RPGDataType] = $RPGDataTypePlayer;
$rpg_data[$RPGPlayerSkill, $RPGDataType] = $RPGDataTypePlayer;
$rpg_data[$RPGPlayerBaseSkill, $RPGDataType] = $RPGDataTypePlayer;
$rpg_data[$RPGPlayerBonusSkill, $RPGDataType] = $RPGDataTypePlayer;
$rpg_data[$RPGClassData, $RPGDataType] = $RPGDataTypeClassData;

//****************************************************************
// fetchData(), storeData(), addBonus(), getBonus(), are used for easy access to Client data
// note: fetchData and storeData will access the base Skills of a client
// note: getRPGData() can be used to access more specific information

function fetchData(%client, %data, %full)
{
	//echo("FETCH DATA->" @ %client @ " " @ %data @ " " @ %full);
	%return_data = "";
	%data_type = $rpg_data[%data, $RPGDataType];
	%return_n = true;
	switch (%data_type) {
		case $RPGDataTypeSkill:
			if (!%full)
				%return_data = getRPGData($RPGPlayerBaseSkill, %client, %data);
			else
				%return_data = getRPGData($RPGPlayerSkill, %client, %data);
		default:
			%return_n = false;
			%return_data = getRPGData($RPGPlayerData, %client, %data);
	}
	if (%return_data $= "")
		if (%return_n)
			return $RPGError;
		else
			return $RPGNull;
	else
		return %return_data;
}

function storeData(%client, %data, %arg_0, %inc)
{
	//echo("STORE DATA->" @ %client @ " " @ %data @ " " @ %arg_0 @ " " @ %inc);
	%data_type = $rpg_data[%data, $RPGDataType];
	switch (%data_type) {
		case $RPGDataTypeSkill:
			setPlayerBaseSkill(%client, %data, %arg_0, %inc);
		default:
			setPlayerData(%client, %data, %arg_0, %inc);
	}
}

function addBonus(%client, %data, %arg_0, %inc)
{
	if ($rpg_data[%data, $RPGDataType] == $RPGDataTypeSkill)
		setPlayerBonusSkill(%client, %data, %arg_0, %inc);
}

function getBonus(%client, %data) 
{
	if ($rpg_data[%data, $RPGDataType] == $RPGDataTypeSkill)
		return getPlayerBonusSkill(%client, %data);
	else
		return $RPGError;
}

//****************************************************************

//****************************************************************
// getRPGData() can be used to access many different types of Data stored on the server.
//
// $RPGPlayerData: used to get Current Client data such as Player Level, EXP, or Skill amount
//		example: getRPGData($RPGPlayerData, client_id, $PlayerEXP);
// $RPGPlayerSkill: used to get Current Skill level, this includes all bonus states for the Skill
//		example: getRPGData($RPGPlayerSkill, client_id, $SkillEnergy);
//		example: getRPGData($RPGPlayerSkill, client_id, $SkillDEF);
//		note: all Skills and Modifiers fall under the namespace of 'Skill' for ease of access
// $RPGPlayerBonusSkill, $RPGPlayerBaseSkill: same as above but can grab just Bonus or Base skill if needed
//		example: getRPGData($RPGPlayerBaseSkill, client_id, $SkillEndurance);
// $RPGClassData: can be used to Grab Class Specific Data
//		example: getRPGData($RPGClassData, $ClassFighter, $RPGDataClassSkillMultiplier, $SkillSlashing);
//		example: getRPGData($RPGClassData, $RaceHuman, $RPGDataClassLifeMultiplier));

function getRPGData(%type, %arg_0, %arg_1, %arg_2)
{
	%return_data = "";
	%data_type = $rpg_data[%type, $RPGDataType];
	switch (%data_type) {
		case $RPGDataTypePlayer:
			switch (%type) {
				case $RPGPlayerData:
					%return_data = getPlayerData(%arg_0, %arg_1);
				case $RPGPlayerSkill:
					%return_data = getPlayerSkill(%arg_0, %arg_1);
				case $RPGPlayerBaseSkill:
					%return_data = getPlayerBaseSkill(%arg_0, %arg_1);
				case $RPGPlayerBonusSkill:
					%return_data = getPlayerBonusSkill(%arg_0, %arg_1);
			}
		case $RPGDataTypeConst:
			%return_data = $rpg_data[%type, $RPGDataConst];
		case $RPGDataTypeClassData:
			%return_data = getClassData(%arg_0, %arg_1, %arg_2);
		default:
			if (%arg_1 $= "")
				%return_data = $rpg_data[%type, %arg_0];
			else
				%return_data = $rpg_data[%type, %arg_0, %arg_1];
	}
	if (%return_data $= "")
		return $RPGNull;
	else
		return %return_data;
}

function setRPGData(%type, %data, %arg_0, %arg_1)
{
	if (%arg_1 $= "")
		$rpg_data[%type, %data] = %arg_0;
	else
		$rpg_data[%type, %data, %arg_0] = %arg_1;
}

function setRPGConst(%type, %arg_0)
{
	$rpg_data[%type, $RPGDataType] = $RPGDataTypeConst;
	$rpg_data[%type, $RPGDataConst] = %arg_0;
	return %type;
}

//****************************************************************

// Exec All RPG Scripts
// NOTES: Execution order matters here to properly set up variables
echo("\c1RPG->\c0Loading RPG Scripts...");
exec("./rpg_codes.cs");
exec("./rpg_skills.cs");
exec("./rpg_class.cs");
exec("./rpg_inventory.cs");
exec("./rpg_wear.cs");
exec("./rpg_item.cs");
exec("./rpg_player.cs");
exec("./rpg_loot.cs");
exec("./rpg_ai.cs");
exec("./rpg_combat.cs");
exec("./rpg_menu.cs");

function test_mount(%id)
{
	%player = %id.player;
	%player.mountImage(LongswordImage, 0);
}

// Default Testing Player, Examples of grabbing RPG Data:
//RPGPlayer::DefaultPlayer(27); // Create a Test Character
//RPGLoot::Test(27); // Give em some Loot
//echo("TEST_PLAYER_DATA:" @ getRPGData($PlayerCoins, $RPGDataText)); // Grab Visible Text Label from $rpg_data
//echo("TEST_CLIENT:" @ getRPGData($RPGPlayerData, 27, $PlayerLVL)); // Grab the Clients Current Level
//echo("TEST_FULL_SKILL:" @ getRPGData($RPGPlayerSkill, 27, $SkillSlashing)); // Grab the Clients Skill Amount (Base + Bonus)
//echo("TEST_BASE_SKILL:" @ getRPGData($RPGPlayerBaseSkill, 27, $SkillSlashing)); // Grab the Clients Base Skill Amount
//echo("TEST_BONUS_SKILL:" @ getRPGData($RPGPlayerBonusSkill, 27, $SkillSlashing)); // Grab the Clients Temporary Bonus Skill Amount
//echo("TEST_IS_BASE_SKILL:" @ getRPGData($SkillSlashing, $RPGDataBaseSkill)); // Check to See if a Skill is a "Base Skill" (Skills that can be added with SP)
//echo("TEST_CODE:" @ getRPGData($SkillSlashing, $RPGDataCode)); // Grab a Skills Code (More on this Later for Mods and Items)
//echo("TEST_DECODE:" @ getRPGData($CodeSlashing, $RPGDataDecode)); // Decode A Skill
//echo("TEST_CLASS_MULTI:" @ getRPGData($RPGClassData, $ClassFighter, $RPGDataClassSkillMultiplier, $SkillSlashing)); // Grab Skill Multi for a Class
//echo("TEST_CLASS_TEXT:" @ getRPGData($ClassFighter, $RPGDataText)); // Grab Visible Text Label from $rpg_data
//echo("TEST_RACE_LIFE_MULTI:" @ getRPGData($RPGClassData, $RaceHuman, $RPGDataClassLifeMultiplier));
//echo("TEST_RACE:" @ getRPGData($RaceHuman, $RPGDataText));


echo("\c1RPG->\c0RPG Load Success.");