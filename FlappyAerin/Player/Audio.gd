extends Node

# Total, this needs to contain a set of available nodes.
# It also needs to have a GENERATE, GET, and RETURN function. 
# Generate will create a new one, then add it to the pool

export(int) var initial_size
var bag = []

func _ready():
	for i in range (initial_size):
		generate()
	
func generate():
	var new_source = AudioStreamPlayer.new()
	add_child(new_source)
	bag.push_back(new_source)

func get_next() -> AudioStreamPlayer:
	var ret: AudioStreamPlayer
	if len(bag) == 0:
		generate()
	ret = bag.pop_back()
	ret.connect("finished", self, "return_to_pool", [ret])
	return ret
	
func return_to_pool(var ret : AudioStreamPlayer):
	ret.stop()
	ret.stream = null
	ret.disconnect("finished", self, "return_to_pool")
	bag.push_back(ret)

func length() -> int:
	return len(bag)
