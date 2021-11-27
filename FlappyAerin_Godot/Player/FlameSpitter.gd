extends Node2D

var fireball = preload("res://Player/Fireball.tscn")

export(int) var initial_size
var bag = []

func _ready():
	for i in range (initial_size):
		generate()
	
func generate():
	var new_source = fireball.instance()
	bag.push_back(new_source)

func get_next() -> Fireball:
	var ret: Fireball
	if len(bag) == 0:
		generate()
	ret = bag.pop_back()
	ret.connect("finished", self, "return_to_pool", [ret])
	return ret
	
func return_to_pool(var ret : Fireball):
	ret.disconnect("finished", self, "return_to_pool")
	bag.push_back(ret)

func length() -> int:
	return len(bag)
