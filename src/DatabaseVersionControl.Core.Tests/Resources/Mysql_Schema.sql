-- MySQL dump 10.13  Distrib 5.1.36, for Win32 (ia32)
--
-- Host: localhost    Database: golfclub_icr_data
-- ------------------------------------------------------
-- Server version	5.1.36-community

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `general_var`
--

DROP TABLE IF EXISTS `general_var`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `general_var` (
  `var_group` varchar(40) NOT NULL DEFAULT '',
  `var_name` varchar(40) NOT NULL DEFAULT '',
  `var_value` mediumtext,
  PRIMARY KEY (`var_group`,`var_name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `general_var`
--

LOCK TABLES `general_var` WRITE;
/*!40000 ALTER TABLE `general_var` DISABLE KEYS */;
/*!40000 ALTER TABLE `general_var` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_finalise_key`
--

DROP TABLE IF EXISTS `icr_finalise_key`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_finalise_key` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `key_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(25) DEFAULT NULL,
  `type` enum('Cash','Cheque','EFT','Account','Hotel XFer') DEFAULT 'Cash',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`key_id`,`create_date`),
  KEY `till_id` (`till_id`,`key_id`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_finalise_key`
--

LOCK TABLES `icr_finalise_key` WRITE;
/*!40000 ALTER TABLE `icr_finalise_key` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_finalise_key` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_finalise_key_sale`
--

DROP TABLE IF EXISTS `icr_finalise_key_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_finalise_key_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `key_id` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`key_id`),
  KEY `till_id` (`till_id`,`key_id`,`create_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_finalise_key_sale`
--

LOCK TABLES `icr_finalise_key_sale` WRITE;
/*!40000 ALTER TABLE `icr_finalise_key_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_finalise_key_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_fixed_total`
--

DROP TABLE IF EXISTS `icr_fixed_total`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_fixed_total` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `fixed_id` int(11) NOT NULL DEFAULT '0',
  `total_name` varchar(24) DEFAULT NULL,
  `total_status` int(11) NOT NULL DEFAULT '0',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`fixed_id`,`create_date`),
  KEY `fixed_id` (`fixed_id`),
  KEY `linknotill` (`fixed_id`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_fixed_total`
--

LOCK TABLES `icr_fixed_total` WRITE;
/*!40000 ALTER TABLE `icr_fixed_total` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_fixed_total` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_fixed_total_sale`
--

DROP TABLE IF EXISTS `icr_fixed_total_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_fixed_total_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `fixed_id` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`fixed_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_fixed_total_sale`
--

LOCK TABLES `icr_fixed_total_sale` WRITE;
/*!40000 ALTER TABLE `icr_fixed_total_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_fixed_total_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_level_names`
--

DROP TABLE IF EXISTS `icr_level_names`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_level_names` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `level_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(25) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`level_id`,`create_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_level_names`
--

LOCK TABLES `icr_level_names` WRITE;
/*!40000 ALTER TABLE `icr_level_names` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_level_names` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu`
--

DROP TABLE IF EXISTS `icr_plu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `plu_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(24) DEFAULT NULL,
  `random_code` varchar(20) DEFAULT NULL,
  `qty_1` double(11,2) DEFAULT NULL,
  `qty_2` double(11,2) DEFAULT NULL,
  `qty_3` double(11,2) DEFAULT NULL,
  `itemizer_1` tinyint(1) DEFAULT NULL,
  `itemizer_2` tinyint(1) DEFAULT NULL,
  `itemizer_3` tinyint(1) DEFAULT NULL,
  `itemizer_4` tinyint(1) DEFAULT NULL,
  `itemizer_5` tinyint(1) DEFAULT NULL,
  `group_id` int(11) DEFAULT NULL,
  `department_id` int(11) DEFAULT NULL,
  `tax_rate` double(11,2) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(19) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`plu_id`,`create_date`),
  KEY `till_id` (`till_id`,`create_date`,`update_date`),
  KEY `create_date` (`create_date`,`update_date`),
  KEY `plu_id` (`plu_id`,`create_date`,`update_date`),
  KEY `group_id` (`group_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu`
--

LOCK TABLES `icr_plu` WRITE;
/*!40000 ALTER TABLE `icr_plu` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_department`
--

DROP TABLE IF EXISTS `icr_plu_department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_department` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `dept_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(25) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`create_date`,`dept_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_department`
--

LOCK TABLES `icr_plu_department` WRITE;
/*!40000 ALTER TABLE `icr_plu_department` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_department` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_department_sale`
--

DROP TABLE IF EXISTS `icr_plu_department_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_department_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `department_id` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`department_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_department_sale`
--

LOCK TABLES `icr_plu_department_sale` WRITE;
/*!40000 ALTER TABLE `icr_plu_department_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_department_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_group`
--

DROP TABLE IF EXISTS `icr_plu_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_group` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `group_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(24) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`group_id`,`create_date`),
  KEY `group_id` (`group_id`),
  KEY `linknotill` (`group_id`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_group`
--

LOCK TABLES `icr_plu_group` WRITE;
/*!40000 ALTER TABLE `icr_plu_group` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_group_sale`
--

DROP TABLE IF EXISTS `icr_plu_group_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_group_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `group_id` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`group_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_group_sale`
--

LOCK TABLES `icr_plu_group_sale` WRITE;
/*!40000 ALTER TABLE `icr_plu_group_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_group_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_price`
--

DROP TABLE IF EXISTS `icr_plu_price`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_price` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `plu_id` int(11) NOT NULL DEFAULT '0',
  `qty` int(11) NOT NULL DEFAULT '0',
  `level` int(11) NOT NULL DEFAULT '0',
  `price` double(11,2) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`plu_id`,`qty`,`level`,`create_date`),
  KEY `till_id` (`till_id`,`plu_id`,`qty`,`level`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_price`
--

LOCK TABLES `icr_plu_price` WRITE;
/*!40000 ALTER TABLE `icr_plu_price` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_price` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_plu_sale`
--

DROP TABLE IF EXISTS `icr_plu_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_plu_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `plu_id` int(11) NOT NULL DEFAULT '0',
  `level` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`plu_id`,`level`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_plu_sale`
--

LOCK TABLES `icr_plu_sale` WRITE;
/*!40000 ALTER TABLE `icr_plu_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_plu_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_price_levels`
--

DROP TABLE IF EXISTS `icr_price_levels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_price_levels` (
  `level_id` int(11) NOT NULL DEFAULT '0',
  `level_name` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`level_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_price_levels`
--

LOCK TABLES `icr_price_levels` WRITE;
/*!40000 ALTER TABLE `icr_price_levels` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_price_levels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_tab`
--

DROP TABLE IF EXISTS `icr_tab`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_tab` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `check_number` int(11) NOT NULL DEFAULT '0',
  `text_name` varchar(40) DEFAULT NULL,
  `open_status` int(11) DEFAULT NULL,
  `clerk_opened` int(11) DEFAULT NULL,
  `location` int(11) DEFAULT NULL,
  `receipt_header1` varchar(255) DEFAULT NULL,
  `receipt_header2` varchar(255) DEFAULT NULL,
  `number_of_lines` int(11) DEFAULT NULL,
  `check_total` double(15,3) DEFAULT NULL,
  `number_of_items` double(15,3) DEFAULT NULL,
  `covers` int(11) DEFAULT NULL,
  `deposit_amount` double(15,3) DEFAULT NULL,
  PRIMARY KEY (`till_id`,`create_date`,`check_number`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_tab`
--

LOCK TABLES `icr_tab` WRITE;
/*!40000 ALTER TABLE `icr_tab` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_tab` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_till`
--

DROP TABLE IF EXISTS `icr_till`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_till` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `till_name` varchar(20) DEFAULT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`create_date`),
  KEY `create_date` (`create_date`,`update_date`),
  KEY `till_id` (`till_id`,`create_date`,`update_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_till`
--

LOCK TABLES `icr_till` WRITE;
/*!40000 ALTER TABLE `icr_till` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_till` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_transaction_key`
--

DROP TABLE IF EXISTS `icr_transaction_key`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_transaction_key` (
  `till_id` int(11) NOT NULL DEFAULT '0',
  `key_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(25) DEFAULT NULL,
  `type` int(11) NOT NULL DEFAULT '0',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `update_number` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`till_id`,`key_id`,`create_date`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_transaction_key`
--

LOCK TABLES `icr_transaction_key` WRITE;
/*!40000 ALTER TABLE `icr_transaction_key` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_transaction_key` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `icr_transaction_key_sale`
--

DROP TABLE IF EXISTS `icr_transaction_key_sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `icr_transaction_key_sale` (
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `till_id` int(11) NOT NULL DEFAULT '0',
  `trans_id` int(11) NOT NULL DEFAULT '0',
  `qty` double(11,3) NOT NULL DEFAULT '0.000',
  `price` double(11,3) NOT NULL DEFAULT '0.000',
  PRIMARY KEY (`create_date`,`till_id`,`trans_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `icr_transaction_key_sale`
--

LOCK TABLES `icr_transaction_key_sale` WRITE;
/*!40000 ALTER TABLE `icr_transaction_key_sale` DISABLE KEYS */;
/*!40000 ALTER TABLE `icr_transaction_key_sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `id_fountain`
--

DROP TABLE IF EXISTS `id_fountain`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `id_fountain` (
  `reference` varchar(20) NOT NULL DEFAULT 'MEMBER_FIELDS',
  `prefix` varchar(10) NOT NULL DEFAULT '',
  `serial_number` int(10) unsigned DEFAULT NULL,
  `display_digits` tinyint(3) unsigned DEFAULT '0',
  PRIMARY KEY (`reference`),
  UNIQUE KEY `reference` (`reference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `id_fountain`
--

LOCK TABLES `id_fountain` WRITE;
/*!40000 ALTER TABLE `id_fountain` DISABLE KEYS */;
/*!40000 ALTER TABLE `id_fountain` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2011-01-02  2:45:13
