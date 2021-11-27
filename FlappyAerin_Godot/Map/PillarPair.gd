#tool
extends Node2D

export(float) var separation = 256

var speed: float = 0

signal finished

func _enter_tree():
	var player_scene = get_node("/root/Node2D/Player")
	var map_scene = get_node("/root/Node2D/Map")
	var score_scene = get_node("/root/Node2D/SCOREUI")
	
	player_scene.connect("player_died", self, "pause")
	$PassArea.connect("body_entered", map_scene, "pass_pillar")
	$PassArea.connect("body_entered", score_scene, "increment_score")

func _physics_process(delta):
	position += Vector2.LEFT * speed * delta
	
	if position.x < -64:
		emit_signal("finished")
	
func reset(s : float):
	separation = s
	$Bottom.position.y = s / 2 + 64 * 3
	$Top.position.y = -s / 2 - 64 * 3
	
	$Box.reset()

func pause():
	speed = 0
