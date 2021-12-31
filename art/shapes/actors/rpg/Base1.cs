
singleton TSShapeConstructor(Base1Dae)
{
   baseShape = "./Base1.dae";
   loadLights = "0";
};

function Base1Dae::onLoad(%this)
{
   %this.setNodeTransform("Mount0", "0.592225 0.0865408 1.0565 0.72543 0.603841 0.330344 1.25491", "1");
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBox-1", "Col-1", "0.0267372 0.169511 -0.151272 0.0181957 -0.999729 0.0145257 1.57107", "0");
   %this.addCollisionDetail("-1", "Box", "Bounds", "4", "30", "30", "32", "30", "30", "30");
   %this.addSequence("./Anims/BaseReady.dae", "root", "0", "-1", "1", "0");
   %this.setSequenceCyclic("root", "0");
   %this.addSequence("./Anims/BaseRun.dae", "run", "1", "10", "1", "0");
}
