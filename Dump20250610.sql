-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: sa
-- ------------------------------------------------------
-- Server version	8.0.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `comment`
--

DROP TABLE IF EXISTS `comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comment` (
  `comment_id` int NOT NULL AUTO_INCREMENT,
  `user_id` varchar(45) NOT NULL,
  `recipe_id` int NOT NULL,
  `content` varchar(80) DEFAULT NULL,
  `created_date` datetime NOT NULL,
  PRIMARY KEY (`comment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comment`
--

LOCK TABLES `comment` WRITE;
/*!40000 ALTER TABLE `comment` DISABLE KEYS */;
INSERT INTO `comment` VALUES (1,'user001',1,'很簡單又好吃！','2024-05-20 11:00:00'),(2,'user002',2,'番茄要選熟一點的比較好','2024-05-22 10:30:00'),(3,'user003',3,'肉絲要先醃一下比較嫩','2024-05-23 14:20:00'),(4,'user001',4,'第一次做成功了！謝謝分享','2024-05-24 19:45:00'),(5,'user004',1,'加個火腿丁會更香','2024-05-25 08:30:00'),(6,'user005',5,'花生米要炸得酥脆一點','2024-05-25 16:15:00'),(7,'user002',3,'我家小孩很愛吃這道菜','2024-05-26 12:45:00'),(8,'user003',1,'晚餐就決定做這個了','2024-05-27 17:30:00'),(9,'user004',2,'這道菜真的很下飯','2024-05-28 13:15:00'),(10,'user005',4,'麻婆豆腐配白飯絕配！','2024-05-29 18:40:00'),(11,'111',2,'123','2025-06-03 17:52:57');
/*!40000 ALTER TABLE `comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `favorite`
--

DROP TABLE IF EXISTS `favorite`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `favorite` (
  `user_id` varchar(45) NOT NULL,
  `recipe_id` int NOT NULL,
  `add_date` datetime NOT NULL,
  PRIMARY KEY (`user_id`,`recipe_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `favorite`
--

LOCK TABLES `favorite` WRITE;
/*!40000 ALTER TABLE `favorite` DISABLE KEYS */;
INSERT INTO `favorite` VALUES ('111',1,'2025-06-02 00:35:11'),('111',2,'2025-06-02 00:35:09'),('111',3,'2025-06-03 17:55:32'),('111',4,'2025-06-02 00:35:12'),('111',5,'2025-06-03 17:55:33'),('user001',1,'2024-05-20 10:30:00'),('user001',3,'2024-05-21 15:45:00'),('user002',2,'2024-05-22 09:15:00'),('user002',4,'2024-05-26 16:30:00'),('user003',4,'2024-05-23 20:10:00'),('user004',5,'2024-05-24 12:00:00'),('user005',1,'2024-05-25 14:20:00');
/*!40000 ALTER TABLE `favorite` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `ingredient_id` int NOT NULL AUTO_INCREMENT,
  `food_name` varchar(45) NOT NULL,
  `quantity` varchar(10) NOT NULL,
  `recipe_id` int NOT NULL,
  PRIMARY KEY (`ingredient_id`),
  UNIQUE KEY `unique_ingredient_recipe` (`food_name`,`recipe_id`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (1,'米飯','2碗',1),(2,'雞蛋','2顆',1),(3,'蔥花','適量',1),(4,'鹽','適量',1),(5,'番茄','3顆',2),(6,'雞蛋','4顆',2),(7,'糖','1茶匙',2),(8,'鹽','適量',2),(9,'豬肉絲','200g',3),(10,'青椒','2顆',3),(11,'醬油','2湯匙',3),(12,'料酒','1湯匙',3),(13,'嫩豆腐','1盒',4),(14,'豬絞肉','100g',4),(15,'豆瓣醬','2湯匙',4),(16,'花椒粉','適量',4),(17,'雞胸肉','300g',5),(18,'花生米','50g',5),(19,'乾辣椒','8根',5),(20,'醋','1湯匙',5),(35,'雞腿肉','300g',21),(36,'九層塔','適量',21),(37,'薑片','5片',21),(38,'蒜頭','3瓣',21),(39,'醬油','2大匙',21),(40,'麻油','1大匙',21),(41,'米酒','2大匙',21),(42,'木耳','適量',22),(43,'紅蘿蔔','半條',22),(44,'豆腐','半塊',22),(45,'蛋','1顆',22),(46,'香菜','少許',22),(47,'白醋','1匙',22),(48,'白胡椒粉','適量',22),(49,'五花肉','200g',23),(50,'蒜苗','1根',23),(51,'辣豆瓣醬','1匙',23),(52,'蒜末','1瓣',23),(53,'醬油','1匙',23),(54,'小黃瓜','2條',24),(55,'鹽','適量',24),(56,'蒜末','1瓣',24),(57,'白醋','1匙',24),(58,'糖','1匙',24),(59,'香油','少許',24),(60,'雞翅','6支',25),(61,'可樂','300ml',25),(62,'醬油','2大匙',25),(63,'薑片','3片',25);
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe`
--

DROP TABLE IF EXISTS `recipe`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe` (
  `recipe_id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(45) NOT NULL,
  `description` varchar(100) NOT NULL,
  `steps` varchar(100) NOT NULL,
  `image` varchar(100) NOT NULL,
  PRIMARY KEY (`recipe_id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe`
--

LOCK TABLES `recipe` WRITE;
/*!40000 ALTER TABLE `recipe` DISABLE KEYS */;
INSERT INTO `recipe` VALUES (1,'蛋炒飯','簡單美味的家常蛋炒飯','1.熱鍋下油 2.炒蛋盛起 3.炒飯加蛋 4.調味起鍋','fried_rice.jpg'),(2,'番茄炒蛋','經典中式家常菜','1.蛋打散 2.番茄切塊 3.先炒蛋 4.下番茄炒勻','tomato_egg.jpg'),(3,'青椒肉絲','下飯的經典菜色','1.肉絲醃製 2.青椒切絲 3.熱鍋炒肉絲 4.加青椒炒勻','pepper_pork.jpg'),(4,'麻婆豆腐','川菜經典麻辣豆腐','1.豆腐切塊 2.肉末炒香 3.加豆腐燒煮 4.勾芡起鍋','mapo_tofu.jpg'),(5,'宮保雞丁','酸甜微辣的川菜','1.雞肉切丁醃製 2.花生米炸香 3.炒雞丁 4.加調料炒勻','kungpao_chicken.jpg'),(21,'三杯雞','香氣撲鼻的經典台式料理','1.雞腿切塊 2.爆香薑蒜辣椒 3.加雞塊炒香 4.加入三杯調味料煮滾收汁','three_cup_chicken.jpg'),(22,'酸辣湯','開胃暖身的中式濃湯','1.食材切絲 2.煮高湯 3.加配料煮滾 4.勾芡加蛋調味','hot_sour_soup.jpg'),(23,'回鍋肉','重口味下飯川菜','1.五花肉煮熟切片 2.炒蒜苗辣豆瓣 3.回鍋炒肉片 4.炒勻出鍋','twice_cooked_pork.jpg'),(24,'涼拌小黃瓜','清爽開胃的冷盤小菜','1.小黃瓜拍碎 2.加鹽靜置脫水 3.調醬汁拌勻 4.冰鎮入味','cucumber_salad.jpg'),(25,'可樂雞翅','甜鹹入味的家常美食','1.雞翅燙過去腥 2.加可樂與醬油燒煮 3.中小火煨煮收汁 4.撒蔥花','cola_chicken.jpg');
/*!40000 ALTER TABLE `recipe` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `user_id` varchar(45) NOT NULL,
  `user_name` varchar(45) NOT NULL,
  `password` varchar(45) NOT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES ('111','test','111111'),('222','222','222222'),('A123','watermelon','123456'),('user001','小明','password123'),('user002','小美','mypassword'),('user003','阿華','secure456'),('user004','麗娟','cook2024'),('user005','大雄','foodlover');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-10 18:35:09
