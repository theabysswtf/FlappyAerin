extends Label

export(Color) var end_color
export(int) var point_threshold = 75

func _ready():
	EventBus.connect("player_died", self, "animate_in")

func animate_in():
	if State.points < point_threshold: return
	
	var tw = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	tw.tween_property(self, "modulate", end_color, 0.5)
