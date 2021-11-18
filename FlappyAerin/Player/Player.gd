extends KinematicBody2D

export(float) var fall_gravity = 640.0
export(float) var glide_gravity = 320.0
export(float) var jump_speed = -160.0
export(int) var max_jump_duration = 250

export(AudioStream) var short_jump
export(AudioStream) var long_jump


var velocity : Vector2
var active : bool
var jumping : bool
var jump_start_time : float
var gravity : float

# Setup
func _ready():
	velocity = Vector2(0,0)
	active = false
	gravity = fall_gravity

# Physics
func _physics_process(delta : float):
	if not active: return
	jumping = jumping and OS.get_ticks_msec() < jump_start_time + max_jump_duration
	if not jumping:
		velocity.y += gravity * delta
	position += velocity * delta

# Input
func _input(event : InputEvent):
	active = true
	if event.is_action_pressed("Jump"): 
		jumping = true
		jump_start_time = OS.get_ticks_msec()
		velocity.y = jump_speed
		gravity = glide_gravity
		play_jump()
	elif event.is_action_released("Jump"): 
		jumping = false
		gravity = fall_gravity
	if event.is_action_pressed("Fire"): fire()
	
func play_jump():
	var source : AudioStreamPlayer = $Audio.get_next()
	source.stream = long_jump
	source.play()
	
func fire():
	print("firing")
