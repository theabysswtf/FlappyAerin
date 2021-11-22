extends KinematicBody2D
class_name Player

export(float) var shot_speed
export(float) var fall_gravity = 640.0
export(float) var glide_gravity = 320.0
export(float) var jump_speed = -160.0
export(float) var max_fall_speed = 480
export(int) var max_jump_duration = 250
export(AudioStream) var jump_sound
export(AudioStream) var hit_sound
export(AudioStream) var boom_sound

var velocity : Vector2
var dead: bool
var active : bool setget set_active
var jumping : bool
var jump_start_time : float
var gravity : float
var scene_anchor : Node

signal player_died

# Setup
func _enter_tree():
	scene_anchor = get_node("/root/Node2D")
	var map_scene = get_node("/root/Node2D/Map")
	connect("player_died", map_scene, "player_died")
	
func _ready():
	velocity = Vector2(0,0)
	set_active(false)
	dead = false
	gravity = fall_gravity
	
	print($Sprite.visible)

# Physics
func _physics_process(delta : float):
	if not active or dead: return
	jumping = jumping and OS.get_ticks_msec() < jump_start_time + max_jump_duration
	if not jumping:
		velocity.y = min(velocity.y + gravity * delta, max_fall_speed)
		
	var collision = move_and_collide(velocity * delta)
	if (collision):
		die()

func die():
	dead = true
	set_active(false)
	emit_signal("player_died")
# Input
func _input(event : InputEvent):
	if dead: return
	
	set_active(true)
	if event.is_action_pressed("Jump"): 
		jumping = true
		jump_start_time = OS.get_ticks_msec()
		velocity.y = jump_speed
		gravity = glide_gravity
		$Audio.play(jump_sound)
		
	elif event.is_action_released("Jump"): 
		jumping = false
		gravity = fall_gravity

	if event.is_action_pressed("Fire"): fire()
	
func fire():
	if (active):
		var new_ball = $FlameSpitter.get_next()
		scene_anchor.add_child(new_ball)
		var y_rate_interp = lerp(5, -5, velocity.y / max_fall_speed)	
		new_ball.global_position = $FlameSpitter.global_position
		new_ball.velocity = Vector2(10, 5).normalized() * shot_speed
		new_ball.activate()

func set_active (a : bool):
	active = a
	
func explosion_impact (grade : int):
	if (grade == 0):
		$Audio.play(hit_sound)
	elif (grade == 1):
		$Audio.play(boom_sound)
