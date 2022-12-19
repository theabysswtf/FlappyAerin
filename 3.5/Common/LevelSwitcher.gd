extends Node2D
var animating: bool

func _ready():
	fade_in()

func fade_in():
	animating = true
	var tw = create_tween().set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_CUBIC)
	tw.tween_property(self, "modulate:a", 0.0, 0.75)
	yield(tw, "finished")
	animating = false
	
func restart():
	if animating:
		return
	animating = true
	var tw = create_tween().set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_CUBIC)
	tw.tween_property(self, "modulate:a", 1.0, 0.2)
	yield(tw, "finished")
	get_tree().change_scene("res://Scenes/World.tscn")
	State.reset()
	GameUI.reset()
	fade_in()
	

	
