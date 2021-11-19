extends Node2D

export(int) var min_msec_between_spawns
export(int) var max_msec_between_spawns
export(int) var spawn_time_reduction_per_increment

export(int) var min_separation
export(int) var max_separation
export(int) var separation_decrease_per_increment

export(int) var max_vertical_offset

export(float) var min_rate_of_motion
export(float) var max_rate_of_motion
export(float) var acceleration_per_increment

export(int) var pillars_per_increment = 4

var obstacle = preload("res://Map/PillarPair.tscn")
var time_of_last_spawn_msec : int
var current_separation : int
var msec_between_spawns : int
var rate_of_motion : float
var pillars_passed : int
var level_active : bool

var bag = []

signal stage_incremented
# Process: Spawn rate is dependent on speed and rate.
# As game progresses, reduce the average vertical separation
# As game progresses, increase spawn rate of pillars

func _enter_tree():
	var score_scene = get_node("/root/Node2D/SCOREUI")
	connect("stage_incremented", score_scene, "increment_score_increment")
	var cam_scene = get_node("/root/Node2D/Camera2D")
	$PillarOrigin.position.y = cam_scene.position.y

func _ready():
	time_of_last_spawn_msec = OS.get_ticks_msec()
	msec_between_spawns = max_msec_between_spawns
	current_separation = max_separation
	rate_of_motion = min_rate_of_motion
	level_active = true
	pillars_passed = 0
	if pillars_per_increment <= 0:
		pillars_per_increment = 4

func _process(_delta: float):
	if level_active and OS.get_ticks_msec() > time_of_last_spawn_msec + msec_between_spawns:
		var current_pillar = get_next()
		
		time_of_last_spawn_msec = OS.get_ticks_msec()
		current_pillar.reset(current_separation)
		current_pillar.speed = rate_of_motion
	
		
		current_pillar.position = $PillarOrigin.position + Vector2.UP * (rand_range(-max_vertical_offset, max_vertical_offset))

func pass_pillar(body : Node):
	if (body is Player):		
		pillars_passed += 1
		if (pillars_passed % pillars_per_increment == 0):
			msec_between_spawns = max(min_msec_between_spawns, msec_between_spawns - spawn_time_reduction_per_increment)
			current_separation = max(min_separation, current_separation - separation_decrease_per_increment)
			rate_of_motion = min(max_rate_of_motion, rate_of_motion + acceleration_per_increment)
			emit_signal("stage_incremented")
			
func player_died():
	level_active = false
		
func generate():
	var new_pillar = obstacle.instance()
	new_pillar.position = $PillarOrigin.position
	add_child(new_pillar)
	bag.push_back(new_pillar)
	
func get_next():
	var ret
	if len(bag) == 0:
		generate()
	ret = bag.pop_back()
	ret.connect("finished", self, "return_to_pool", [ret])
	return ret
	
func return_to_pool(var ret):
	ret.disconnect("finished", self, "return_to_pool")
	bag.push_back(ret)

func length() -> int:
	return len(bag)
