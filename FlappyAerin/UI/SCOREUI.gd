extends Control
class_name SCOREUI

export (NodePath) var ScoreText
export (int) var score_increment
export (int) var score_increment_increment
export (AudioStream) var score_sound
export (AudioStream) var stage_sound

var score : int
var scoreText : RichTextLabel

func _ready():
	score = 0
	scoreText = get_node(ScoreText)
	scoreText.text = "Score: %d" % score

func increment_score(body : Node):
	if (body is Player):		
		score += score_increment
		scoreText.text = "Score: %d" % score
		$Audio.play(score_sound)

func increment_score_increment():
	score_increment += score_increment_increment
	$Audio.play(stage_sound)
