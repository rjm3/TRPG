
// All ShapeBase images are mounted into one of 8 slots on a shape.  This weapon
// system assumes all primary weapons are mounted into this specified slot:
$WeaponSlot = 0;

//-----------------------------------------------------------------------------
// Weapon Class
//-----------------------------------------------------------------------------

function RPGWeapon::onUse(%data, %obj)
{
    rpgecho($RPGEchoWeapon,"RPGWeapon::onUse",%data,%obj);
    // Default behavior for all weapons is to mount it into the object's weapon
   // slot, which is currently assumed to be slot 0
   if (%obj.getMountedImage($WeaponSlot) != %data.image.getId())
   {
      serverPlay3D(WeaponUseSound, %obj.getTransform());
      %obj.mountImage(%data.image, $WeaponSlot);
      if (%obj.client)
      {
         if (%data.description !$= "")
            messageClient(%obj.client, 'MsgWeaponUsed', '\c0%1 selected.', %data.description);
         else
            messageClient(%obj.client, 'MsgWeaponUsed', '\c0Weapon selected');
      }
      
      // If this is a Player class object then allow the weapon to modify allowed poses
      if (%obj.isInNamespaceHierarchy("Player"))
      {
         // Start by allowing everything
         %obj.allowAllPoses();
         
         // Now see what isn't allowed by the weapon
         
         %image = %data.image;
         
         if (%image.jumpingDisallowed)
            %obj.allowJumping(false);
         
         if (%image.jetJumpingDisallowed)
            %obj.allowJetJumping(false);
         
         if (%image.sprintDisallowed)
            %obj.allowSprinting(false);
         
         if (%image.crouchDisallowed)
            %obj.allowCrouching(false);
         
         if (%image.proneDisallowed)
            %obj.allowProne(false);
         
         if (%image.swimmingDisallowed)
            %obj.allowSwimming(false);
      }
   }
   
}

function RPGWeapon::onPickup(%this, %obj, %shape, %amount)
{
    rpgecho($RPGEchoWeapon,"RPGWeapon::onPickup",%this,%obj,%shape,%amount);
   // The parent Item method performs the actual pickup.
   // For player's we automatically use the weapon if the
   // player does not already have one in hand.
   if (Parent::onPickup(%this, %obj, %shape, %amount))
   {
      serverPlay3D(WeaponPickupSound, %shape.getTransform());
      if (%shape.getClassName() $= "Player" && %shape.getMountedImage($WeaponSlot) == 0)
         %shape.use(%this);
   }
}

function RPGWeapon::onInventory(%this, %obj, %amount)
{
    rpgecho($RPGEchoWeapon,"RPGWeapon::onInventory",%this,%obj,%amount);
   // Weapon inventory has changed, make sure there are no weapons
   // of this type mounted if there are none left in inventory.
   if (!%amount && (%slot = %obj.getMountSlot(%this.image)) != -1)
      %obj.unmountImage(%slot);
}

//-----------------------------------------------------------------------------
// Weapon Image Class
//-----------------------------------------------------------------------------

function MeleeWeaponImage::onMount(%this, %obj, %slot)
{
    rpgecho($RPGEchoWeapon,"MeleeWeaponImage::onMount",%this,%obj,%slot);
    if (%obj.client !$= "" && !%obj.isAiControlled)
         %obj.client.RefreshWeaponHud(%currentAmmo, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %amountInClips);
}

function MeleeWeaponImage::onUnmount(%this, %obj, %slot)
{
    rpgecho($RPGEchoWeapon,"MeleeWeaponImage::onUnmount",%this,%obj,%slot);
    if (%obj.client !$= "" && !%obj.isAiControlled)
      %obj.client.RefreshWeaponHud(0, "", "");
}

//$WeaponRange['Longsword'] = 1;
//Not used yet
function GetRange(%weapon)
{
	rpgecho($RPGEchoWeapon,"GetRange",%weapon);

	%minRange = 2.0;
	//if($WeaponRange[%weapon] != "")
		return %minRange + $WeaponRange[%weapon];
	//else
	//	return %minRange + $RangeTable[$AccessoryVar[%weapon, $AccessoryType]];
}

$fireTimeDelay = 0.1;
$meleeBump = 40000.0;
function MeleeAttack(%player,%length,%weapon)
{
    rpgecho($RPGEchoWeapon,"MeleeAttack",%player,%length,%weapon);
    
    %clientId = %player.getControllingClient();
    
    //==== ANTI-SPAM CHECK, CAUSE FOR SPAM UNKNOWN ==========
    %time = getSimTime() >> 5;
    if(%time - %clientId.lastFireTime <= $fireTimeDelay)
		return;
	%clientId.lastFireTime = %time;
	//=======================================================
    
    %look = %player.getLookAtPoint(%length,$TypeMasks::PlayerObjectType);
    
    echo("Look At: " @ %look);
    
    %dmgObj = getWords(%look,0,0);
    
    %dirVec = %player.getEyeVector();//VectorNormalize(VectorSub(%dmgObj.getPosition(),%player.getPosition()));
    echo(%dirVec);
    %dmgObj.applyDamage(30);
    //%force = VectorScale(%dirVec,$meleeBump);
    //echo(%force);
    //%dmgObj.applyImpulse(%player.getPosition(),%force);
    
    
}

// ----------------------------------------------------------------------------
// A "generic" weaponimage onFire handler for most weapons.  Can be overridden
// with an appropriate namespace method for any weapon that requires a custom
// firing solution.

// projectileSpread is a dynamic property declared in the weaponImage datablock
// for those weapons in which bullet skew is desired.  Must be greater than 0,
// otherwise the projectile goes straight ahead as normal.  lower values give
// greater accuracy, higher values increase the spread pattern.
// ----------------------------------------------------------------------------

function MeleeWeaponImage::onFire(%this, %player, %slot)
{
    rpgecho($RPGEchoWeapon,"MeleeWeaponImage::onFire",%this,%player,%slot);
    if(%this.hitDetectRaycast)
    {
        MeleeAttack(%player,%this.meleeRange,%this.weaponName);
    }
}

// ----------------------------------------------------------------------------
// A "generic" weaponimage onAltFire handler for most weapons.  Can be
// overridden with an appropriate namespace method for any weapon that requires
// a custom firing solution.
// ----------------------------------------------------------------------------

function MeleeWeaponImage::onAltFire(%this, %obj, %slot)
{
    rpgecho($RPGEchoWeapon,"MeleeWeaponImage::onAltFire",%this,%obj,%slot);
}

//Fire underwater
function MeleeWeaponImage::onWetFire(%this, %obj, %slot)
{
    rpgecho($RPGEchoWeapon,"MeleeWeaponImage::onWetFire",%this,%obj,%slot);
    %this.onFire(%obj,%slot);
}

// ----------------------------------------------------------------------------
// Weapon cycling
// ----------------------------------------------------------------------------

function ShapeBase::clearWeaponCycle(%this)
{
   %this.totalCycledWeapons = 0;
}

function ShapeBase::addToWeaponCycle(%this, %weapon)
{
   %this.cycleWeapon[%this.totalCycledWeapons++ - 1] = %weapon;
}

function ShapeBase::cycleWeapon(%this, %direction)
{
   // Can't cycle what we don't have
   if (%this.totalCycledWeapons == 0)
      return;
      
   // Find out the index of the current weapon, if any (not all
   // available weapons may be part of the cycle)
   %currentIndex = -1;
   if (%this.getMountedImage($WeaponSlot) != 0)
   {
      %curWeapon = %this.getMountedImage($WeaponSlot).item.getName();
      for (%i=0; %i<%this.totalCycledWeapons; %i++)
      {
         if (%this.cycleWeapon[%i] $= %curWeapon)
         {
            %currentIndex = %i;
            break;
         }
      }
   }

   // Get the next weapon index
   %nextIndex = 0;
   %dir = 1;
   if (%currentIndex != -1)
   {
      if (%direction $= "prev")
      {
         %dir = -1;
         %nextIndex = %currentIndex - 1;
         if (%nextIndex < 0)
         {
            // Wrap around to the end
            %nextIndex = %this.totalCycledWeapons - 1;
         }
      }
      else
      {
         %nextIndex = %currentIndex + 1;
         if (%nextIndex >= %this.totalCycledWeapons)
         {
            // Wrap back to the beginning
            %nextIndex = 0;
         }
      }
   }
   
   // We now need to check if the next index is a valid weapon.  If not,
   // then continue to cycle to the next weapon, in the appropriate direction,
   // until one is found.  If nothing is found, then do nothing.
   %found = false;
   for (%i=0; %i<%this.totalCycledWeapons; %i++)
   {
      %weapon = %this.cycleWeapon[%nextIndex];
      if (%weapon !$= "" && %this.hasInventory(%weapon) && %this.hasAmmo(%weapon))
      {
         // We've found out weapon
         %found = true;
         break;
      }
      
      %nextIndex = %nextIndex + %dir;
      if (%nextIndex < 0)
      {
         %nextIndex = %this.totalCycledWeapons - 1;
      }
      else if (%nextIndex >= %this.totalCycledWeapons)
      {
         %nextIndex = 0;
      }
   }
   
   if (%found)
   {
      %this.use(%this.cycleWeapon[%nextIndex]);
   }
}