// RPG PLAYER

$player_data_master_list = "";

function getPlayerData(%client, %data)
{
	return $rpg_player_data[%client, %data];
}

function getPlayerSkill(%client, %data)
{
	return $rpg_player_base_skill[%client, %data] + $rpg_player_bonus_skill[%client, %data];
}

function getPlayerBaseSkill(%client, %data)
{
	return $rpg_player_base_skill[%client, %data];
}

function getPlayerBonusSkill(%client, %data)
{
	return $rpg_player_bonus_skill[%client, %data];
}

function setPlayerData(%client, %data, %arg_0, %inc)
{
	if (%inc)
		$rpg_player_data[%client, %data] += %arg_0;
	else
		$rpg_player_data[%client, %data] = %arg_0;
}

function setPlayerBaseSkill(%client, %data, %arg_0, %inc)
{
	if (%inc)
		$rpg_player_base_skill[%client, %data] += %arg_0;
	else
		$rpg_player_base_skill[%client, %data] = %arg_0;
}

function setPlayerBonusSkill(%client, %data, %arg_0, %inc)
{
	if (%inc)
		$rpg_player_bonus_skill[%client, %data] += %arg_0;
	else
		$rpg_player_bonus_skill[%client, %data] = %arg_0;
}

function RPGPlayer::AddData(%data, %text, %code)
{
	$player_data_master_list = $player_data_master_list @ %data @ " ";
	setRPGData(%data, $RPGDataType, $RPGDataTypePlayerData);
	setRPGData(%data, $RPGDataText, %text);
	setRPGData(%data, $RPGDataCode, %code);
	setRPGData(%code, $RPGDataDecode, %data);
	return %data;
}

// Add Player Data Types
$PlayerLVL = RPGPlayer::AddData("player_lvl", "Level", $CodeLVL);
$PlayerRemort = RPGPlayer::AddData("player_remort", "Remort", $CodeRemort);
$PlayerEXP = RPGPlayer::AddData("player_exp", "EXP", $CodeEXP);
$PlayerCoins = RPGPlayer::AddData("player_coins", "Coins", $CodeCoins);
$PlayerClass = RPGPlayer::AddData("player_class", "Class", $CodeClass);
$PlayerRace = RPGPlayer::AddData("player_race", "Race", $CodeRace);

function RPGPlayer::DefaultPlayer(%client)
{
	%list = $player_data_master_list;
	for (%i = 0; %i < getWordCount(%list); %i++) {
		%data = getWord(%list, %i);
		setPlayerData(%client, %data, 0);
	}
	setPlayerData(%client, $PlayerLVL, 1);
	%list = $skill_data_master_list;
	for (%i = 0; %i < getWordCount(%list); %i++) {
		%skill = getWord(%list, %i);
		setPlayerBaseSkill(%client, %skill, 8);
		setPlayerBonusSkill(%client, %skill, 0);
	}
	RPGWear::DefaultWear(%client);
	RPGInventory::DefaultInventory(%client);
	RPGLoot::Test(%client);
	setPlayerData(%client, $RPGPlayerPickingItem, $RPGNull);
}

function RPGPlayer::DefaultInventory(%client)
{
	rpgecho($RPGEchoPlayer, "RPGPlayer::DefaultInventory", %client);
	RPGInventory::DefaultInventory(%client);
}

echo("\c1RPG->\c0Player Loaded.");