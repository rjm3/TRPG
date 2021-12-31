// RPG Client Tab Menu

setClientData($RPGClientTabMenuGui, "TabMenu");
setClientData($RPGClientShowTabMenu, false);

TabMenu.setVisible(false);
$CLIENT_MENU_OPTION_NULL = 0;
$CLIENT_MENU_OPTION_COMMAND = 1;
$CLIENT_MENU_OPTION_SUB_COMMAND = 2;
$CLIENT_MENU_OPTION_TEXT = 3;
$client_tab_menu_header = "";
$client_tab_menu_type = "";

function clientCmdNewMenu(%menu_type, %menu_header)
{
	echo("clientCmdNewMenu:" @ %menu_type @ ":" @ %menu_header);
	$client_tab_menu_header = %menu_header;
	$client_tab_menu_type = %menu_type;
	RPGClient::UpdateMenu();
}

function clientCmdAddOption(%option_id, %command, %sub_command, %text)
{
	echo("clientCmdAddOption:" @ %option_id @ ":" @ %command @ ":" @ %sub_command @ ":" @ %text);
	$client_tab_menu_option[%option_id, $CLIENT_MENU_OPTION_COMMAND] = %command;
	$client_tab_menu_option[%option_id, $CLIENT_MENU_OPTION_SUB_COMMAND] = %sub_command;
	$client_tab_menu_option[%option_id, $CLIENT_MENU_OPTION_TEXT] = %text;
	%option = "option_" @ %option_id;
	%option.setText(%text);
	%option.visible = true;
}

function RPGClient::menuRequest()
{
	RPGClient::defaultMenu();
	commandToServer('MenuRequest');
}

function RPGClient::defaultMenu()
{
	$client_tab_menu_header = "";
	for (%i = 0; %i <= 27; %i++)
		$client_tab_menu_option[%i, $CLIENT_MENU_OPTION_TEXT] = $CLIENT_MENU_OPTION_NULL;
		$client_tab_menu_option[%i, $CLIENT_MENU_OPTION_COMMAND] = $CLIENT_MENU_OPTION_NULL;
		$client_tab_menu_option[%i, $CLIENT_MENU_OPTION_SUB_COMMAND] = $CLIENT_MENU_OPTION_NULL;
	RPGClient::updateMenu();
}

function RPGClient::updateMenu()
{
	echo("RPGClient::updateMenu");
	Header.setText($client_tab_menu_header);
	for (%i = 0; %i <= 27; %i++) {
		%option = "option_" @ %i;
		%option.setText($client_tab_menu_option[%i, $OPTION_TEXT]);
		%option.visible = false;
	}
}

function RPGClient::selectTabOption(%option)
{
	echo("RPGClient::selectTabOption:" @ %option);
	if ($client_tab_menu_option[%option, $CLIENT_MENU_OPTION_COMMAND] != $CLIENT_MENU_OPTION_NULL) {
		%type = $client_tab_menu_type;
		%command = $client_tab_menu_option[%option, $CLIENT_MENU_OPTION_COMMAND];
		%sub_command = $client_tab_menu_option[%option, $CLIENT_MENU_OPTION_SUB_COMMAND];
		commandToServer('SelectTabOption', %type, %command, %sub_command);
	}
}

function clientToggleTabMenu(%val)
{
	if (%val) {
		%mouse_outside_gui = getClientData($RPGClientMouseOutsideGui);
		if (getClientData($RPGClientShowTabMenu)) {
			TabMenu.setVisible(false);
			setClientData($RPGClientShowTabMenu, false);
			hideCursor();
		}
		else {
			%mouse_outside_gui.setVisible(false);
			TabMenu.setVisible(true);
			RPGClient::menuRequest();
			setClientData($RPGClientShowTabMenu, true);
			showCursor();
		}
	}
}

echo("\c4RPG_Client->\c0Tab Menu Loaded");