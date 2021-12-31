// RPG MENU

function serverCmdMenuRequest(%client)
{
	echo("serverCmdMenuRequest:" @ %client);
	RPGMenu::MenuRequest(%client);
}

function serverCmdSelectTabOption(%client, %type, %command, %sub_command)
{
	echo("serverCmdSelectTabOption:" @ %client @ ":" @ %type @ ":" @ %command @ ":" @ %sub_command);
	RPGMenu::ProcessOption(%client, %type, %command, %sub_command);
}

$SERVER_MENU_TYPE_DEFAULT = 0;
$SERVER_MENU_TYPE_OPTIONS = 1;
$SERVER_MENU_TYPE_SKILLS = 2;

$SERVER_MENU_OPTION_NULL = 0;
$SERVER_MENU_OPTION_VIEW_STATS = 1;
$SERVER_MENU_OPTION_REQUEST_OPTIONS = 2;
$SERVER_MENU_OPTION_REQUEST_SKILLS = 3;
$SERVER_MENU_OPTION_ADD_SKILL = 4;
$SERVER_MENU_OPTION_GO_BACK = 5;

function RPGMenu::MenuRequest(%client)
{
	echo("RPGMenu::MenuRequest:" @ %client);
	commandToClient(%client, 'NewMenu', $SERVER_MENU_TYPE_DEFAULT, "RPG Menu");
	%i = 0;
	commandToClient(%client, 'AddOption', %i, $SERVER_MENU_OPTION_VIEW_STATS, $SERVER_MENU_OPTION_NULL, "View Your Stats..");
	commandToClient(%client, 'AddOption', %i++, $SERVER_MENU_OPTION_REQUEST_OPTIONS, $SERVER_MENU_OPTION_NULL, "Options..");
	commandToClient(%client, 'AddOption', %i++, $SERVER_MENU_OPTION_REQUEST_SKILLS, $SERVER_MENU_OPTION_NULL, "Skills..");
	//commandToClient(%client, 'NewMenu', $SERVER_MENU_TYPE_DEFAULT, "RPG Menu");
	//for (%i = 0; %i <= 27; %i++) {
	//	commandToClient(%client, 'AddOption', %i, $SERVER_MENU_OPTION_VIEW_STATS, $SERVER_MENU_OPTION_NULL, "Option" @ %i);	
	//}
}

function RPGMenu::ProcessOption(%client, %type, %command, %sub_command)
{
	echo("RPGMenu::ProcessOption:" @ %client @ ":" @ %type @ ":" @ %command @ ":" @ %sub_command);
	switch (%type) {
		case $SERVER_MENU_TYPE_DEFAULT:
			switch (%command) {
				case $SERVER_MENU_OPTION_VIEW_STATS:
					echo("VIEW STATS");
				case $SERVER_MENU_OPTION_REQUEST_OPTIONS:
					echo("REQUEST OPTIONS");
				case $SERVER_MENU_OPTION_REQUEST_SKILLS:
					RPGMenu::RequestSkills(%client);
		}
		case $SERVER_MENU_TYPE_SKILLS:
			switch (%command) {
				case $SERVER_MENU_OPTION_ADD_SKILL:
					RPGMenu::AddSkill(%client, %sub_command);
				case $SERVER_MENU_OPTION_GO_BACK:
					RPGMenu::MenuRequest(%client);
			}
	}
}

function RPGMenu::RequestSkills(%client)
{
	echo("RPGMenu::RequestSkills:" @ %client);
	commandToClient(%client, 'NewMenu', $SERVER_MENU_TYPE_SKILLS, "You have 0 Skills Points");
	%i = 0;
	commandToClient(%client, 'AddOption', %i, $SERVER_MENU_OPTION_ADD_SKILL, $SKILL_SLASHING, "(1.5) Slashing");
	commandToClient(%client, 'AddOption', %i++, $SERVER_MENU_OPTION_ADD_SKILL, $SKILL_ENDURANCE, "(1.0) Endurance");
	commandToClient(%client, 'AddOption', %i++, $SERVER_MENU_OPTION_ADD_SKILL, $SKILL_ENERGY, "(0.5) Energy");
	commandToClient(%client, 'AddOption', %i++, $SERVER_MENU_OPTION_GO_BACK, $SERVER_MENU_OPTION_NULL, "<< Back");
}

function RPGMenu::AddSkill(%client, %skill)
{
	echo("RPGMenu::AddSkill:" @ %client @ ":" @ %skill);
}