// RPG CLASS

$class_data_master_list = "";

function RPGClass::AddClass(%class_name, %text, %code, %super)
{
	setRPGData(%class_name, $RPGDataType, $RPGDataTypeClass);
	setRPGData(%class_name, $RPGDataText, %text);
	setRPGData(%class_name, $RPGDataCode, %code);
	setRPGData(%code, $RPGDataDecode, %class_name);
	if (%super !$= "")
		setRPGData(%class_name, $RPGDataSuperClass, %super);
	$class_data_master_list = $class_data_master_list @ %class_name @ " ";
	return %class_name;
}

function setClassData(%class_name, %type, %arg_0, %arg_1)
{
	if (%arg_1 !$= "")
		$rpg_class_data[%class_name, %type, %arg_0] = %arg_1;
	else
		$rpg_class_data[%class_name, %type] = %arg_0;
}

function getClassData(%class_name, %type, %arg_0)
{
	if (%arg_0 !$= "")
		return $rpg_class_data[%class_name, %type, %arg_0];
	else
		return $rpg_class_data[%class_name, %type];
}

// Race Data
$RaceHuman = RPGClass::AddClass("race_human", "Human", $CodeHuman);
setClassData($RaceHuman, $RPGDataClassLifeMultiplier, 0.7);
// Super Class Data
$ClassWarrior = RPGClass::AddClass("class_warrior", "Warrior", $CodeWarrior);
// Sub Class Data
$ClassFighter = RPGClass::AddClass("class_fighter", "Fighter", $CodeFighter, $SuperClassWarrior);
setClassData($ClassFighter, $RPGDataClassSkillMultiplier, $SkillSlashing, 1.5);
setClassData($ClassFighter, $RPGDataClassSkillMultiplier, $SkillEndurance, 2.0);
setClassData($ClassFighter, $RPGDataClassSkillMultiplier, $SkillEnergy, 0.2);
setClassData($ClassFighter, $RPGDataClassLifeMultiplier, 1.8);

echo("\c1RPG->\c0Class Loaded.");