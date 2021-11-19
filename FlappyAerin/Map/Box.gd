extends Area2D

var active : bool

var player_scene : Node

signal impact(i)
	
func _ready():
	active = true
	player_scene = get_node("/root/Node2D/Player")
	connect("impact", player_scene, "explosion_impact")
	
	
func _on_Box_body_shape_entered(body_id, body, body_shape, local_shape):
	if body is Fireball:
		$Particles2D.emitting = true
		$Sprite.visible = false
		active = false
		emit_signal("impact", 1)
	if body is Player and active:
		var player = body as Player
		player.die()

func reset():
	active = true
	$Sprite.visible = true
	$Particles2D.emitting = false
