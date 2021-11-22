extends KinematicBody2D
class_name Fireball

var velocity : Vector2
var player_scene : Node

var ret_func : FuncRef
var active : bool

signal impact(i)
signal finished

func _enter_tree():
	player_scene = get_node("/root/Node2D/Player")
	connect("impact", player_scene, "explosion_impact")
	
func activate():
	#$Trail.initial_velocity = velocity.length()
	#$Trail.direction = velocity / $Trail.initial_velocity
	$Sprite.visible = true
	$Smoke.emitting = false
	$Trail.emitting = true
	
	active = true
	
func _physics_process(delta):
	if not active: return
	
	var collision = move_and_collide(velocity * delta)
	if (collision):
		emit_signal("impact", 0)
		$Sprite.visible = false
		$Smoke.emitting = true
		$Trail.emitting = false
		
		active = false
		yield(get_tree().create_timer($Smoke.lifetime * 2), "timeout")
		get_parent().remove_child(self)
		collision.collider.add_child(self)
		
		emit_signal("finished")
		get_parent().remove_child(self)
		
func _exit_tree():
	disconnect("impact", player_scene, "explosion_impact")
