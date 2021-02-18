# TNGC15-Project
This project is a simple ray tracer written in C#, as part of the course Global Illumination and Rendering


It's functional but unfinnished.

Known Issues:
- Surfaces close or directly under to the light source seem to be recieving too much light and results in 
too much intensity (>1.0f) at the surface which, since we scale pixel channel values according to the largest intensity,
the entire image gets darkend.
Cause could be that multiple samples(rays from camera) hit the light early in the ray path.

- The points on objects which aren't in direct light become black and don't emit their actual color. A temporary fix was implemented until
a proper solution could be implemented. We added a line that made it so that if point is blocked from direct
light then it should emit the surface color rather than pitch black. This means that when the point is getting indirect light
from surrounding surfaces, the point will gain intensity showing some of the true surface color.

- Light triangles normals work as intended even though they are reversed. There is some term in some function that's
inverted for this to not cause a problem. Not sure where. Should be investigated.
