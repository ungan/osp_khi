const express = require("express");
const session = require("express-session");
const app = express();
const path = require("path");
const cors = require("cors");

const bodyParser = require("body-parser");
const cookieParser = require("cookie-parser");

process.env.NODE_ENV = "production";
//process.env.NODE_ENV = "development";
console.log("env->" + process.env.NODE_ENV);

app.set("trust proxy", 1); // trust first proxy
app.use(cors());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(cookieParser());

app.use(
  session({
    secret: "leshanchor",
    resave: true,
    saveUninitialized: true,
    cookie: {
      httpOnly: false,
      secure: false,
      expires: new Date(Date.now() + 2 * 60 * 60 * 1000),
      maxAge: 2 * 60 * 60 * 1000, //2h
    },
  })
);
app.use("/labeling/users", require("./routes/users"));
app.use("/labeling/admin", require("./routes/admin"));
app.use("/labeling/api", require("./routes/myapi"));

// Serve static assets if in production
process.env.PORT = 9910;
// Set static folder
app.use("/labeling", express.static("./checker/build"));

app.get("/*", function (req, res) {
  res.sendFile(path.resolve("./checker/build", "index.html"));
});

const port = process.env.PORT || 4000;

app.listen(port, () => {
  console.log(`Server Running at ${port}`);
});
