// RPG AI

function RPGAI::Spawn() 
{
    //338.668 287.218 104.269
	%ai_player = AIPlayer::spawnAtLocation("GoblinRunt","338.668 287.218 106.269"); //"-923 -2697 175.5");
    echo(%ai_player);
	RPGPlayer::DefaultPlayer(%ai_player);
	%ai_player.mountImage(LurkerWeaponImage, 0);
	%ai_player.setInventory(LurkerAmmo, 128);
}

function RPGAI::Think(%player)
{
	return;
	//echo("RPGAI::Think", %player);
	// Grab the Nearest Target by Client Group Index Id
	%get_target = AIPlayer::getNearestPlayerTarget(%player);
	if (%get_target == -1) {
		%player.setImageTrigger(0, false);
		return;
	}
	%player.setImageTrigger(0, true);
	%target_client = ClientGroup.getObject(%get_target);
	%target_object = %target_client.player;
	%player.aimAt(%target_object);
	%target_pos = %target_object.getPosition();
	%player_pos = %player.getPosition();
	%distance = VectorDist(%target_pos, %player_pos);
	%target_transform = %target_object.getTransform();
	%target_pos_x = getWord(%target_transform, 0);
	%target_pos_y = getWord(%target_transform, 1);
	%target_pos_z = getWord(%target_transform, 2);
	// Make the AI stand a little bit away from the Target, needs much improvement here
	%move_transform = %target_pos_x + 1 @ " " @ %target_pos_y + 1 @ " " @ %target_transform_z @ " 0 0 0";
	if (%distance >= 2)
		%player.setMoveDestination(%move_transform, 1);
}

echo("\c1RPG->\c0AI Loaded.");