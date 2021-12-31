// RPG COMBAT

function RPGCombat::Damage(%attacker, %victim, %damage)
{
	echo("RPG::Damage " @ %attacker.client @ " " @ %victim @ " " @ %damage);
	%victim_name = %victim.getShapeName();
	RPG::Msg(%attacker.client, " You hit " @ %victim_name @ " for " @ %damage @ " damage!");
}

echo("\c1RPG->\c0Combat Loaded.");