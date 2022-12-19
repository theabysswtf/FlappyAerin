extends Node2D

export(float, 0, 1) var scroll_speed

var shaders: Array
var weights: Array
var offsets: Array

var t: float

func _ready():
	shaders = []
	weights = []
	
	for node in get_tree().get_nodes_in_group("Parallax"):
		var shader = node.material as ShaderMaterial
		var weight = float(String(node.name).split("_")[1]) / 10.0
		shaders.push_back(shader)
		weights.push_back(weight)
		offsets.push_back(0)

func _process(delta): 
	for i in range(shaders.size()):
		offsets[i] += weights[i] * delta * scroll_speed
		shaders[i].set_shader_param("offset", Vector2(offsets[i], 0.0))
