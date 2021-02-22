# TNGC15-Project
This project uses object-oriented programming in C# to create a simple monte carlo ray tracer, as part of the course Global Illumination and Rendering.

![raytraced_image25spp_A](https://github.com/Nfrederiksen/TNGC15-Project/blob/master/TNCG15-simple-ray-tracer/bin/Debug/net471/ppnngg/rayTracedImage480x480_01.png?raw=true)
[Normal]
![raytraced_image25spp_B](https://github.com/Nfrederiksen/TNGC15-Project/blob/master/TNCG15-simple-ray-tracer/bin/Debug/net471/ppnngg/rayTracedImage480x480_11.png?raw=true)
[w/ hot fix]
![raytraced_image25spp_C](https://github.com/Nfrederiksen/TNGC15-Project/blob/master/TNCG15-simple-ray-tracer/bin/Debug/net471/ppnngg/rayTracedImage480x480_11_enhanced.png?raw=true)
[w/ contrast added]

It's functional but unfinnished.

![raytraced_image25spp_X](https://github.com/Nfrederiksen/TNGC15-Project/blob/master/TNCG15-simple-ray-tracer/bin/Debug/net471/ppnngg/rayTracedImage_lightdistanceFault.png?raw=true)

Known Issues:
- Surfaces close or directly under to the light source seem to be recieving too much light and results in 
too much intensity (>1.0f) at the surface which, since we scale pixel channel values according to the largest intensity,
the entire image gets darkend.
Cause seems to be tied to the geometic term, the lightDistance term. Smaller lightDistance values create wild geometric term values. But we followed the formula in the Lecture slides. Perhaps we misread something or didn't implement it correctly.

- The points on objects which aren't in direct light become black and don't emit their actual color. A temporary fix was implemented until
a proper solution could be implemented. We added a line that made it so that if point is blocked from direct
light then it should emit the surface color rather than pitch black. This means that when the point is getting indirect light
from surrounding surfaces, the point will gain intensity showing some of the true surface color.

- Light triangles normals work as intended even though they are reversed. There is some term in some function that's
inverted for this to not cause a problem. Not sure where. Should be investigated.
