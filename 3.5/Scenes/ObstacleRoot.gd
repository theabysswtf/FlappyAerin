extends Node2D

export(PackedScene) var small_obstacle_tscn
export(PackedScene) var big_obstacle_tscn
#----------------------------------------------
export(int) var base_msec_to_next_obstacle
export(int) var msec_reduction_per_step
export(int) var msec_minimum
#----------------------------------------------
export(Vector2) var start_velocity
export(Vector2) var acceleration_step
#----------------------------------------------
export(int) var max_v_separation # i.e. 196
export(int) var min_v_separation # i.e. 128  Delta = 68
export(int) var levels_to_min_v_separation # i.e. 5, will reduce amount by delta/level
#----------------------------------------------
export(int) var max_v_position # i.e. 128
export(int) var min_v_position # i.e. -128
export(int) var max_v_step # i.e. 196
#----------------------------------------------

var msec_of_last_obstacle: int
var last_v_position: float
var emitting: bool

func _ready():
	EventBus.connect("game_started", self, "_on_game_started")
	EventBus.connect("player_died", self, "_on_game_ended")

func _on_game_started():
	emitting = true
	
func _on_game_ended():
	emitting = false

func _physics_process(delta):
	if not State.alive: return
	var msec_to_next_obstacle: float = max(base_msec_to_next_obstacle - msec_reduction_per_step * State.level, msec_minimum)
	if Time.get_ticks_msec() > msec_of_last_obstacle + msec_to_next_obstacle and emitting:
		spawn_obstacle()
		msec_of_last_obstacle = Time.get_ticks_msec()
		
	var inst_velocity = start_velocity + acceleration_step * State.level
	for obstacle in get_children():
		# Move to the side by the expected amount
		if obstacle is Obstacle:
			move_obstacle(obstacle, inst_velocity, delta)

func spawn_obstacle():
	# big or small?
	var size = randi() % 2 + 1
	var width = 96*size
	
	# Handle vertical separation
	var vs_step = min(State.level, levels_to_min_v_separation)
	var vs_reduction = (max_v_separation - min_v_separation) / levels_to_min_v_separation * vs_step
	var v_separation = max_v_separation - vs_reduction
	
	# Handle vertical offset
	var t: float = last_v_position + randi() % (max_v_step * 2) - max_v_step
	var clamped_v_position: float = clamp(t, min_v_position, max_v_position)
	last_v_position = clamped_v_position
	
	var tscn: PackedScene = small_obstacle_tscn if size == 1 else big_obstacle_tscn
	var new_obstacle = tscn.instance() as Obstacle
	add_child(new_obstacle)
	new_obstacle.init(width, v_separation, clamped_v_position)
	

func move_obstacle(obstacle: Obstacle, inst_velocity: Vector2, delta: float):
	obstacle.position += inst_velocity * delta
	if obstacle.global_position.x < - obstacle.width * 4:
		obstacle.call_deferred("queue_free")

func reset():
	emitting = false
	for child in get_children():
		child.set_deferred("queue_free")
