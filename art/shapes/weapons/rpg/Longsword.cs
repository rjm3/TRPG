
singleton TSShapeConstructor(LongswordDae)
{
   baseShape = "./Longsword.dae";
   loadLights = "0";
};

function LongswordDae::onLoad(%this)
{
   %this.setNodeTransform("blade", "0.00243528 4.65661e-10 -0.137265 -0.995899 0.0634749 -0.0644616 1.4745", "1");
   %this.setNodeTransform("Armature", "0.000420725 0 0 1 0 0 0", "1");
   %this.setNodeTransform("Bone", "0.00243528 -0.0576431 -0.105 -1 0 0 1.43345", "0");
   %this.setNodeParent("blade", "Bone");
}
