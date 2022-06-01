
singleton TSShapeConstructor(LongswordDae)
{
   baseShape = "./Longsword.dae";
   loadLights = "0";
};

//Not needed unless 3rd person player animation should change based on weapon mounted
//%this.addSequence("art/shapes/weapons/rpg/Longsword.DAE fire", "ls_fire");
//%this.addSequence("art/shapes/weapons/rpg/Longsword.DAE run", "ls_run");
//%this.addSequence("art/shapes/weapons/rpg/Longsword.DAE idle", "ls_idle");
//%this.setSequenceCyclic("ls_fire", "0");

function LongswordDae::onLoad(%this)
{
   %this.renameSequence("ambient", "timeline");
   %this.addSequence("timeline", "root", "0", "0", "1", "0");
   %this.addSequence("timeline", "Run", "0", "0", "1", "0");
   %this.addSequence("timeline", "fire", "0", "20", "1", "0");
   %this.setSequenceCyclic("fire", "0");
   %this.addSequence("timeline", "idle", "0", "0", "1", "0");
   %this.setNodeTransform("blade", "0.00243528 4.65661e-10 -0.137265 -0.995899 0.0634749 -0.0644616 1.4745", "1");
   %this.setNodeTransform("Armature", "0.000420725 0 0 1 0 0 0", "1");
   %this.setNodeTransform("Bone", "0.00243528 -0.0576431 -0.105 -1 0 0 1.43345", "0");
   %this.setNodeParent("blade", "Bone");
   %this.setSequenceCyclic("timeline", "0");
}
