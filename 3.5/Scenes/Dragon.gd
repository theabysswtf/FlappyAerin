extends KinematicBody2D

export(int) var _max_fall_speed
export(int) var _max_fall_anim_frame
export(int) var _msec_to_next_flap
export(int) var _msec_max_glide_time
export(int) var _fall_frame_count
export(float) var _flap_force
export(float) var _flight_animation_cutoff
export(float) var _glide_gravity
export(float) var _gravity

var _flap_player: AudioStreamPlayer
var _sprite: AnimatedSprite

var _msec_of_last_flap: int
var _velocity: Vector2
var _gliding: bool
var _flying: bool
var _spawn_location: Vector2

func _ready() -> void:
	_sprite = $AnimatedSprite as AnimatedSprite
	_flap_player = $FlapPlayer as AudioStreamPlayer
	_spawn_location = position
	EventBus.connect("player_died", self, "_on_player_died")
	reset()

func _input(event) -> void:
	if event.is_action_released("flap"):
		_gliding = false
	if event.is_action_pressed("flap"):
		_gliding = true
		_flap()
	if not State.alive and event.is_action_pressed("flap"):
		LevelSwitcher.restart()
		
func _flap() -> void:
	if Time.get_ticks_msec() > _msec_of_last_flap + _msec_to_next_flap and State.alive:
		_sprite.frame = 0
		_velocity.y = _flap_force
		_sprite.play("flap")
		_flap_player.play()
		_msec_of_last_flap = Time.get_ticks_msec()
		if not _flying: 
			_flying = true
			EventBus.emit_signal("game_started")
		

func _process(_delta) -> void:
	# what's the dealio here? if velocity > the cutoff and not playing, use this.
	if _velocity.y > _flight_animation_cutoff and State.alive:
		_sprite.playing = false 
		var t = clamp(_velocity.y, _flight_animation_cutoff, _max_fall_anim_frame)
		t -= _flight_animation_cutoff
		t /= _flight_animation_cutoff + _max_fall_anim_frame
		t = (1-t) * (_fall_frame_count)
		_sprite.frame = t
	

func _physics_process(delta) -> void:
	if not State.alive: return
	if not _flying: return
	var _collision = move_and_collide(_velocity * delta)
	var frame_gravity: float = _gravity
	if _gliding and Time.get_ticks_msec() < _msec_of_last_flap + _msec_max_glide_time:
		frame_gravity = _glide_gravity
	_velocity.y = min(_velocity.y + frame_gravity * delta, _max_fall_speed)

func _on_player_died() -> void:
	_sprite.stop()
	_sprite.visible = false
	$DeathParticles.emitting = true
	$BloodParticles.emitting = true
	$DeathPlayer.play()

func reset() -> void:
	_sprite.play("idle")
	position = _spawn_location
