extends Node2D

export(int) var msec_between_spawns
export(int) var min_separation
export(int) var max_separation
export(int) var initial_count
export(float) var rate_of_motion

var obstacle = preload("res://Map/PillarPair.tscn")
var time_of_last_spawn_msec : int
var current_separation : int

var bag = []
# Process: Spawn rate is dependent on speed and rate.
# As game progresses, reduce the average vertical separation
# As game progresses, increase spawn rate of pillars

func _ready():
	time_of_last_spawn_msec = OS.get_ticks_msec()
	current_separation = max_separation

func _process(delta: float):
	if OS.get_ticks_msec() > time_of_last_spawn_msec + msec_between_spawns:
		var current_pillar = get_next()
		current_pillar.speed = rate_of_motion
		current_pillar.set_separation(current_separation)
		time_of_last_spawn_msec = OS.get_ticks_msec()
	
		rate_of_motion += .1
		msec_between_spawns -= 10
		current_separation = max(min_separation, current_separation - 1)
		current_position.position = $PillarOrigin.position + Vector2.UP * (randf(256) - 128)
		
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
