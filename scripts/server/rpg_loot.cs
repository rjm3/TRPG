// RPG Loot

function RPGLoot::AddPack(%pack, %item)
{
	return;
}

function RPGLoot::Test(%client)
{
	rpgecho($RPGEchoLoot, "RPGLoot::Test", %client);
	for (%i = 0; %i <= 6; %i++) {
		%item = RPGItem::Create($BroadSword, 1);
		%add_ok = RPGInventory::Add(%client, 0, %item);
		if (!%add_ok)
			echo("You're no Beast of Burden!");
	}
	%item = RPGItem::Create($PaddedArmor, 1);
	%add_ok = RPGInventory::Add(%client, 0, %item);
	if (!%add_ok)
		echo("You're no Beast of Burden!");
}

echo("\c1RPG->\c0Loot Loaded.");