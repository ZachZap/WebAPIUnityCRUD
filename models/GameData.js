var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var GameData = new Schema({
    gamename:{
        type:String,
        required:true
    },
    gamestudio:{
        type:String,
        required:true
    }
});

mongoose.model('games', GameData);