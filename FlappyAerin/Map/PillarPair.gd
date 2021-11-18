#tool
extends Node2D

export(float) var separation = 256

var speed: float = 0

signal finished

func _physics_process(delta):
	position += Vector2.LEFT * speed
	
	if position.x < -64:
		emit_signal("finished")
	
func set_separation(s : float):
	separation = s
	$Bottom.position.y = s / 2 + 64 * 3
	$Top.position.y = -s / 2 - 64 * 3
