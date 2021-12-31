// RPG SKILLS

$skill_data_master_list = "";
$skill_data_base_master_list = "";

function RPGSkills::AddData(%skill, %text, %code, %base_skill)
{
	setRPGData(%skill, $RPGDataType, $RPGDataTypeSkill);
	setRPGData(%skill, $RPGDataName, %skill);
	setRPGData(%skill, $RPGDataCode, %code);
	setRPGData(%code, $RPGDataDecode, %skill);
	if (%base_skill) {
		setRPGData(%skill, $RPGDataBaseSkill, true);
		$skill_data_base_master_list = $skill_data_base_master_list @ %skill @ " ";
	}
	setRPGData(%skill, $RPGIsSkillData, true);
	$skill_data_master_list = $skill_data_master_list @ %skill @ " ";
	setRPGData(%skill, $RPGDataText, %text);
	return %skill;
}

// Add Base Skills that can be increased with SP
$SkillSlashing = RPGSkills::AddData("skill_slashing", "Slashing", $CodeSlashing, true);
$SkillEndurance = RPGSkills::AddData("skill_endurance", "Endurance", $CodeEndurance, true);
$SkillEnergy = RPGSkills::AddData("skill_energy", "Energy", $CodeEnergy, true);
// Add Modifier Skills that can be increased with other sources (example: Items or Spells)
$SkillATK = RPGSkills::AddData("skill_atk", "Attack Rating", $CodeATK);
$SkillDEF = RPGSkills::AddData("skill_def", "Defense Rating", $CodeDEF);
$SkillMDEF = RPGSkills::AddData("skill_mdef", "Magic Resist", $CodeMDEF);

function RPGSkills::AddSkillPoint(%client, %skill, %modifier)
{
	echo("RPGSkills::AddSkillPoint:" @ %client @ ":" @ %skill @ ":" @ %modifier);
}

echo("\c1RPG->\c0Skills Loaded.");