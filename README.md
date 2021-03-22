# Path Tracer

Simple path tracer created for my Advanced Computer Graphics class.

## Renders

![Lambertian example](renders/sphere_light_lambertian.png "Lambertian example")

![Oren-Nayar vs Lambertian example](renders/oren-nayar_0.9_vs_lambertian.png "Oren-Nayar vs Lambertian example")

## Features

* Path tracing
	* Russian roulette stopping
	* Importance sampling
* Spherical lights
	* Configurable emission side (inner, outer, both) `SideEnum`
* Oren-Nayar BRDF material
	* Configurable roughness parameter `sigma`
