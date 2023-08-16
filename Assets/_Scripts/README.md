# Planet Generation
The original code was from the Sebastian Lague´s tutorial "Procedural Planets".
Modified from Léon-Lucas Kaniewski @ SAE "Generische Welten" Online Lecture.
Modified and finalized by me, for my own project.

## Noise Settings (Folder)
- In the Noise Settings (also Simple and Ridgid), I extracted the nested classes and made them their own files. 
- I added extensions to reuse code for the NoiseFilter classes.
- I also added a few comments to the code. I tried to get rid of the "conditional hide property drawer" class, 
with making the Simple and Ridgid inherit from a abstract class, but the classes then didnt appear in the inspector as expected.
- In the Noise Settings (also Simple and Ridgid), I extracted the nested classes and made them their own files. 
- I added extensions to reuse code for the NoiseFilter classes.
- I also added a few comments to the code. I tried to get rid of the "conditional hide property drawer" class, 
with making the Simple and Ridgid inherit from a abstract class, but the classes then didnt appear in the inspector as expected.

## Surface Generation (Folder)
- SurfaceElevationGradient.cs: 
  - I did minimal adjustments and added the SaveTextureToFile Extension.
- SurfaceShape.cs:
  - I did minimal adjustments and tried another algorithm for the SeamlessEdges method but i couldn't see the difference.
  - I added the option to generate a simple sphere with the "NormalizedSphere" method.
- SurfaceShapeDispatcher.cs:
  - A new class to dispatch the SurfaceShape methods to the TerrainFace class and set the chosen shape in the editor.

## ObjectGenerator.cs
  - I added Properties from the ObjectSettings class to the ObjectGenerator class, so that other classes can access them 
    without having to pass the ObjectSettings class around.
  - I added a method to remove the generated mesh from the gameobject, so it can be regenerated.
  - In the Initialize method, I added a check if there are already child meshes on the gameobject, so that they can be reused,
    instead of generating new ones. If there are no child meshes, the method will generate new ones.
  - I added the GetOrAddComponent2 method to the gameobject, so that the ObjectGenerator class can add the MeshFilter and MeshRenderer
    components to the gameobject, if they are not already present. The unity implementation didn't work for me.
  - I added and changed some Update methods, so that the ObjectGenerator class can update the mesh, if the settings are changed in the editor.

## TerrainFace.cs
  - I added the SurfaceShapeDispatcher.GetSurfaceShape to the TerrainFace class, so that the SurfaceShape methods can be dispatched to the TerrainFace class.