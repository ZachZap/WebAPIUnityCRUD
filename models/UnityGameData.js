var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var UnityGameData = new Schema({
    score:{
        type:Number,
        required:true
    },
    dateJoined:{
        type:String,
        required:true
    },
    firstName:{
        type:String,
        required:true
    },
    lastName:{
        type:String,
        required:true
    },
    userName:{
        type:String,
        required:true
    }
});

mongoose.model('unityplayers', UnityGameData);