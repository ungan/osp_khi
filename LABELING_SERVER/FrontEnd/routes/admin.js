const express = require("express");
const router = express.Router();
const axios = require("axios");
const config = require("./config/key");
const moment = require("moment");
var d = new Date();
var fmt1 = 'YYYY.MM.DD HH:mm:ss';
var now = moment(d).format(fmt1); //Date 객체를 파라미터로 넣기

const radikoAPI = config.radikoURI + "/admin";
//=================================
//             admin
//=================================

router.post("/user", (req, res) => {
  const apiMethod = "user";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          user_total: apires.data.user_total,
          worker_total: apires.data.worker_total,
          verifier_total: apires.data.verifier_total,
          admin_total: apires.data.admin_total,
          normal_total: apires.data.normal_total,
          list: apires.data.list,
        });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          success: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else {
        return res.json({ success: false, message: apires.result.message });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/auth", (req, res) => {
  const apiMethod = "auth";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      list: req.body.list,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
        });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          success: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else {
        return res.json({ success: false, message: apires.result.message });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/statistics", (req, res) => {
  const apiMethod = "statistics";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          image_total: apires.data.image_total,
          image_total_pr: apires.data.image_total_pr,
          image_total_hw: apires.data.image_total_hw,
          image_cnt_typo: apires.data.image_cnt_typo,
          label_cnt_pr: apires.data.label_cnt_pr,
          verify_1_cnt_pr: apires.data.verify_1_cnt_pr,
          verify_2_cnt_pr: apires.data.verify_2_cnt_pr,
          admin_cnt_pr: apires.data.admin_cnt_pr,
          label_cnt_hw: apires.data.label_cnt_hw,
          verify_1_cnt_hw: apires.data.verify_1_cnt_hw,
          verify_2_cnt_hw: apires.data.verify_2_cnt_hw,
          admin_cnt_hw: apires.data.admin_cnt_hw,
        });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          success: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/approval", (req, res) => {
  const apiMethod = "approval";
  let pos = 0;
  if (req.body.pos !== undefined) {
    pos = req.body.pos;
  }

  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      date:req.body.date,
      type:req.body.type,
      style:req.body.style,
      pos: parseInt(pos),
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
            success: true,
            approval_count: apires.data.approval_count,
            image_id: apires.data.image_id,
            image: apires.data.image,
            image_width: apires.data.image_width,
            image_height: apires.data.image_height,
            image_text: apires.data.image_text,
            label_data: apires.data.label_data,
            pos: apires.data.pos,
          });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          success: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else if (apires.result.status === "225") {
        //검수된 이미지 없음.
        if (apires.data.pos > apires.data.approval_count) {
          return res.status(200).json({
              success: false,
              approval_count: apires.data.approval_count,
              pos: apires.data.approval_count,
              message: apires.result.message,
            });
        } else {
          return res.status(200).json({
            success: false,
            approval_count: apires.data.approval_count,
            pos: apires.data.pos,
            message: apires.result.message,
          });
        }
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/reject", (req, res) => {
  const apiMethod = "reject";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      no:req.body.no,
      image_id: req.body.image_id,
      type:req.body.type,
      style:req.body.style,
      cause: req.body.cause,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          approval_count: apires.data.approval_count,
          typo_count: apires.data.typo_count,
          where: req.body.where,
          pos: req.body.pos,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/confirm", (req, res) => {
  const apiMethod = "confirm";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      image_id: req.body.image_id,
      check: req.body.check,
      typo: req.body.typo,
      feedback: req.body.feedback,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          verified_count: apires.data.verified_count,
          typo_count: apires.data.typo_count,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/feedback", (req, res) => {
  const apiMethod = "feedback";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      image_id: req.body.image_id,
      member_id: req.body.member_id,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          verified_count: apires.data.verified_count,
          pos: req.body.pos,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/typo", (req, res) => {
  const apiMethod = "typo";
  let wrongPos = 0;
  if (req.cookies.admin_wrong_pos !== undefined) {
    wrongPos = req.cookies.admin_wrong_pos;
  }
  if (req.body.pos !== undefined) {
    wrongPos = req.body.pos;
  }
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      pos: parseInt(wrongPos),
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res
          .cookie("admin_wrong_pos", apires.data.pos, {
            expires: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
          })
          .status(200)
          .json({
            success: true,
            no:apires.data.no,
            typo_count: apires.data.typo_count,
            image_id: apires.data.image_id,
            image: apires.data.image,
            image_width: apires.data.image_width,
            image_height: apires.data.image_height,
            image_text: apires.data.image_text,
            type: apires.data.type,
            style: apires.data.style,
            typo_user: apires.data.typo_user,
            typo_user_name: apires.data.typo_user_name,
            pos: apires.data.pos,
          });
      } else if (
        apires.result.status === "202" ||
        apires.result.status === "203"
      ) {
        return res.clearCookie("w_auth").status(200).json({
          success: false,
          status: apires.result.status,
          message: apires.result.message,
        });
      } else if (apires.result.status === "225") {
        //검수된 이미지 없음.
        return res.status(200).json({
          success: false,
          typo_count: apires.data.typo_count,
          pos: apires.data.pos,
          message: apires.result.message,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/payment_list", (req, res) => {
  const apiMethod = "payment_list";
  let dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
  };
  if (req.body.id !== undefined) {
    dataToSubmit["data"]["id"] = req.body.id;
  }
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          calclist: apires.data.list,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/payment", (req, res) => {
  const apiMethod = "payment";
  let dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: {
      id: req.body.id,
      pay_type: parseInt(req.body.pay_type),
      pay_cnt: parseInt(req.body.pay_cnt),
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          calclist: apires.data.list,
        });
      } else {
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [admin "+apiMethod+"]>"+now+"/"+ err));
});

module.exports = router;
