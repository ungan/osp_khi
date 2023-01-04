const express = require("express");
const router = express.Router();
const axios = require("axios");
const config = require("./config/key");
const moment = require("moment");
var d = new Date();
var fmt1 = 'YYYY.MM.DD HH:mm:ss';
var now = moment(d).format(fmt1); //Date 객체를 파라미터로 넣기

const radikoAPI = config.radikoURI + "/member";

//=================================
//             User
//=================================

router.post("/auth", (req, res) => {
  if (req.session.user) {
    return res.status(200).json({
      userId: req.session.user.userId,
      level: req.session.user.level,
      job_type: req.session.user.job_type,
      job_style: req.session.user.job_style,
      name: req.session.user.name,
      token:req.session.user.token,
    });
  } else {
    return res.status(200).json({
      message: "토큰이 없습니다!",
    });
  }
});

router.post("/register", (req, res) => {
  const apiMethod = "join";
  const dataToSubmit = {
    method: apiMethod,
    data: {
      id: req.body.email,
      pwd: req.body.password,
      name: req.body.name,
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
      } else {
        return res.json({ success: false, message: apires.result.message });
      }
    })
    .catch((err) => console.log("err [users "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/login", (req, res) => {
  const apiMethod = "login";
  var dataToSubmit = {};
  // if (req.body.w_auth) {
  //   dataToSubmit = {
  //     method: apiMethod,
  //     token: req.body.w_auth,
  //     data: {
  //       id: "",
  //       pwd: "",
  //     },
  //   };
  // } else if (req.body.email && req.body.password) {
  dataToSubmit = {
    method: apiMethod,
    token: "",
    data: {
      id: req.body.email,
      pwd: req.body.password,
      job_type: req.body.job_type,
    },
  };
  // } else {
  //   console.log("what else");
  // }

  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        req.session.user = {
            userId: req.body.email,
            level: apires.data.level,
            job_type: apires.data.job_type,
            job_style: apires.data.job_style,
            name: apires.data.name,
            token:apires.data.token,
        };
        return res
          .cookie("w_auth", apires.data.token, {
            expires: new Date(Date.now() + 24 * 60 * 60 * 1000),
          })
          .status(200)
          .json({
            loginSuccess: true,
            userId: req.body.email,
            level: apires.data.level,
            job_type: apires.data.job_type,
            job_style: apires.data.job_style,
            name: apires.data.name,
          });
      } else {
        let apierror = apires.result.message;
        return res.json({ loginSuccess: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [users "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/logout", (req, res) => {
  const apiMethod = "logout";
  var dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        req.session.destroy();
        return res.clearCookie("w_auth").status(200).json({
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
        let apierror = apires.result.message;
        return res.json({ success: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [users "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/autologout", (req, res) => {
  req.session.destroy();
  return res.clearCookie("w_auth").status(200).json({
    success: true,
  });
});

router.post("/findpw", (req, res) => {
  const apiMethod = "find_pwd";
  const dataToSubmit = {
    method: apiMethod,
    data: {
      id: req.body.email,
    },
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          isChange: true,
          np: apires.data.temp_pwd,
        });
      } else {
        let apierror = apires.result.message;
        return res.status(200).json({ isChange: false, message: apierror });
      }
    })
    .catch((err) => console.log("err [users "+apiMethod+"]>"+now+"/"+ err));
});

router.post("/change_info", (req, res) => {
  const apiMethod = "change_info";
  const dataToSubmit = {
    method: apiMethod,
    token: req.cookies.w_auth,
    data: req.body.data,
  };
  axios
    .post(radikoAPI, dataToSubmit)
    .then(function (response) {
      let apires = response.data;
      if (apires.result.message === "ok" && apires.result.status === "000") {
        return res.status(200).json({
          success: true,
          is_token_expired: apires.data.is_token_expired,
          data: req.body.data,
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
    .catch((err) => console.log("err [users "+apiMethod+"]>"+now+"/"+ err));
});
module.exports = router;
