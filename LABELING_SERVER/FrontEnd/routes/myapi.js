const express = require("express");
const router = express.Router();
const axios = require("axios");
const config = require("./config/key");
const radikoAPI = config.radikoURI + "/data";
const fs = require("fs");
const moment = require("moment");
var d = new Date();
var fmt1 = 'YYYY.MM.DD HH:mm:ss';
var now = moment(d).format(fmt1); //Date 객체를 파라미터로 넣기
//=================================
//             myapi
//=================================

router.post("/getData", (req, res) => {
  const apiMethod = "get";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      type: req.body.type,
      style:req.body.style
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          getDataSuccess: true,
          no:apires.data.no,
          image_style: apires.data.style,
          image_type: apires.data.type,
          image_id: apires.data.image_id,
          image_text: apires.data.image_text,
          image: apires.data.image,
          image_width: apires.data.image_width,
          image_height: apires.data.image_height,
          verified_count: apires.data.verified_count,
          label_data: apires.data.label_data,
          data_status: apires.data.status,
          status: apires.result.status,
        });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          getDataSuccess: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else {
        return res.status(200).json({
          getDataSuccess: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      }
    })
    .catch((err) => console.log("err [myapi "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/upload", (req, res) => {
  const apiMethod = "upload";

  let dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      no:req.body.no,
      image_id: req.body.id,
      type: req.body.type,
      style:req.body.style,
      status: req.body.status,
      word: req.body.word,
      sentence:req.body.sentence,
      chk1_modify: req.body.chk1_modify,
      chk2_dpi: req.body.chk2_dpi,
      chk3_text_meta: req.body.chk3_text_meta,
      chk4_boundbox: req.body.chk4_boundbox,
      chk5_size: req.body.chk5_size,
      chk6_meta_label: req.body.chk6_meta_label,
      chk7_text_label: req.body.chk7_text_label,
      chk8_bound_label: req.body.chk8_bound_label,
      label_data: req.body.label_data,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          sendDataSuccess: true,
          image_id: apires.data.image_id,
          verified_count: apires.data.verified_count,
        });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          sendDataSuccess: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else {
        return res.json({
          sendDataSuccess: false,
          message: apires.result.message,
        });
      }
    })
    .catch((err) => console.log("err [myapi "+apiMethod+"]>"+now+"/"+ err));
});

module.exports = router;
