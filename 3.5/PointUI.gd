extends Node2D

var point_label: Label

func _ready():
	point_label = $CenterContainer/PanelContainer/Label as Label
	EventBus.connect("point_scored", self, "_on_point_scored")
	
func _on_point_scored(point_value) -> void:
	# Create temporary label at the correct spot
	yield(get_tree().create_timer(0.5), "timeout")
	point_label.text = String(State.points)

func reset():
	_on_point_scored(0)
