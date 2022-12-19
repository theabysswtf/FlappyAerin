extends Node

export(int) var level_threshold = 15
export(int) var hint_threshold = 75

var alive: bool
var points: int
var point_player: AudioStreamPlayer
var phase_player: AudioStreamPlayer

var level: int

func _ready() -> void:
	alive = true
	point_player = $PointPlayer
	phase_player = $PhasePlayer
	EventBus.connect("point_scored", self, "_on_point_scored")
	EventBus.connect("player_died", self, "_on_player_died")

func reset() -> void:
	alive = true
	points = 0
	level = 0
	
func _on_player_died() -> void:
	alive = false
	
func _on_point_scored(point_value) -> void:
	points += point_value
	var old_level = level
	level = int(points / level_threshold)
	if old_level != level:
		phase_player.play()
	else:
		point_player.play()
