// RPG CODES
// The purpose of Codes is to help break Items and Save Data into Strings for easier Storing

function getCode(%data)
{
	return getRPGData(%data, $RPGDataCode);
}

function getDecode(%code)
{
	return getRPGData(%code, $RPGDataDecode);
}

$CodeNull = "NUL";

// Player Data
$CodeLVL = "LVL";
$CodeRemort = "REM";
$CodeEXP = "EXP";
$CodeCoins = "CON";
$CodeClass = "CLL";
$CodeRace = "RAC";

// Skill Data
$CodeSlashing = "SLA";
$CodeEndurance = "END";
$CodeEnergy = "ENG";
$CodeATK = "ATK";
$CodeDEF = "DEF";
$CodeMDEF = "MDF";

// Class Data
$CodeHuman = "HUM";
$CodeWarrior = "WAR";
$CodeFighter = "FIG";

echo("\c1RPG->\c0Codes Loaded.");